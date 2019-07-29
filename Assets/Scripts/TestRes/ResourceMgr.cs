using System.Collections;
using System.Collections.Generic;
using Assets.Framework.Singleton;
using UnityEngine;

public enum LoadResPriority
{
    RES_HIGHT = 0,//最高优先级
    RES_MIDDLE,//一般优先级
    RES_SLOW,//低优先级
    RES_NUM,
}

public class AsyncLoadResParam
{

}


public class ResourceMgr : Singleton<ResourceMgr>
{
    /// <summary>
    /// 是否从AB包加载
    /// </summary>
    public bool m_loadFromAssetBundle = false;
    /// <summary>
    /// 缓存引用计数为零的资源列表，达到缓存最大的时候释放这个列表里面最早没用的资源
    /// </summary>
    protected CMapList<ResourceItem> m_NoRefrenceAssetMapList = new CMapList<ResourceItem>();
    /// <summary>
    /// 记录一下正在使用的ResourceItem
    /// </summary>
    public Dictionary<uint, ResourceItem> AssetDic { get; set; } = new Dictionary<uint, ResourceItem>();

    //最长连续卡着加载资源的时间，单位微秒
    private const long MAXLOADRESTIME = 200000;

    //最大缓存个数
    private const int MAXCACHECOUNT = 500;

    //Mono脚本
    protected MonoBehaviour m_Startmono;

    protected List<AsyncLoadResParam>[] m_LoadingAssetList = new List<AsyncLoadResParam>[(int) LoadResPriority.RES_NUM];

    public void Init(MonoBehaviour mono)
    {
        for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
        {
            m_LoadingAssetList[i] = new List<AsyncLoadResParam>();
        }
        m_Startmono = mono;
        m_Startmono.StartCoroutine(AsyncLoadCor());
    }

    public T LoadResource<T>(string path) where T : UnityEngine.Object
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        uint crc = Crc32.GetCrc32(path);
        ResourceItem item = GetCacheResourceItem(crc);
        if (item != null)
        {
            return item.m_Obj as T;
        }

        T obj = null;
#if UNITY_EDITOR
        if (!m_loadFromAssetBundle)//编辑器加载
        {
            obj = LoadAssetByEditor<T>(path);
            item = AssetBundleMgr.Instance.GetResourceItem(crc);
        }
#endif
        if (obj == null)
        {
            item = AssetBundleMgr.Instance.LoadResouceAssetBundle(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
            }
        }

        CacheResource(path,ref item, crc, obj);
        return obj;

    }

    void CacheResource(string path,ref ResourceItem item, uint crc, Object obj,int addrefcount=1)
    {
        WashOut();
        if (item == null)
        {
            Debug.LogError("ResourceItem is null " + path);
            
        }

        if (obj == null)
        {
            Debug.LogError("ResourceLoad Fail " + path);
        }
        item = new ResourceItem();
        item.m_Guid = obj.GetInstanceID();
        item.m_LastUseTime = Time.realtimeSinceStartup;
        item.Retain(addrefcount);
        ResourceItem oldItem = null;
        if (AssetDic.TryGetValue(item.m_Crc, out oldItem))//更新
        {
            AssetDic[item.m_Crc] = item;
        }
        else
        {
            AssetDic.Add(item.m_Crc, item);
        }
    }


#if UNITY_EDITOR
    protected T LoadAssetByEditor<T>(string path) where T : UnityEngine.Object
    {
        return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
    }
#endif

    ResourceItem GetCacheResourceItem(uint crc, int addrefcount = 1)
    {
        ResourceItem item = null;
        if (AssetDic.TryGetValue(crc, out item))
        {
            if (item != null)
            {
                item.Retain(addrefcount);
                item.m_LastUseTime = Time.realtimeSinceStartup;
                //if (item.RefCount <= 1)
                //{
                //    m_NoRefrenceAssetMapList.Remove(item);
                //}
            }
        }

        return item;
    }

    protected void DestoryResourceItem(ResourceItem item, bool destroyCache = false)
    {
        if (item == null || item.RefCount > 0)
        {
            return;
        }

        if (!destroyCache)
        {
            m_NoRefrenceAssetMapList.InsertToHead(item);
            return;
        }

        if (!AssetDic.Remove(item.m_Crc))
        {
            return;
        }

        m_NoRefrenceAssetMapList.Remove(item);

        //释放assetbundle引用
        AssetBundleMgr.Instance.ReleaseAsset(item);

        //清空资源对应的对象池
        //ObjectMgr.Instance.ClearPoolObject(item.m_Crc);

//        if (item.m_Obj != null)
//        {
//            item.m_Obj = null;
//#if UNITY_EDITOR
//            Resources.UnloadUnusedAssets();
//#endif
//        }
    }


    public bool ReleaseResource(Object obj,bool destroyObj=false)
    {
        if (obj == null)
        {
            return false;
        }

        ResourceItem item = null;
        foreach (var res in AssetDic.Values)
        {
            if (res.m_Guid == obj.GetInstanceID())
            {
                item = res;
            }
        }

        if (item == null)
        {
            Debug.LogError("AssetDic里不存在该资源" + obj.name + "可能释放了多次");
            return false;
        }
        
        item.Release();
        DestoryResourceItem(item);
        return true;
    }

    /// <summary>
    /// 缓存太多，清除最早没有使用的资源
    /// </summary>
    protected void WashOut()
    {
        //当大于缓存个数时，进行一半释放
        while (m_NoRefrenceAssetMapList.Size() >= MAXCACHECOUNT)
        {
            for (int i = 0; i < MAXCACHECOUNT / 2; i++)
            {
                ResourceItem item = m_NoRefrenceAssetMapList.Back();
                DestoryResourceItem(item, true);
            }
        }
    }

    IEnumerator AsyncLoadCor()
    {
        while (true)
        {
            yield return null;
        }
    }


}


