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



public delegate void OnLoadResFinish(string path, Object obj, object param1 = null, object param2 = null,
    object param3 = null);

public class ResourceMgr : Singleton<ResourceMgr>
{
    /// <summary>
    /// 是否从AB包加载
    /// </summary>
    public bool m_loadFromAssetBundle = true;
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
    //异步加载队列
    protected List<AsyncLoadResInfo>[] m_LoadingAssetList = new List<AsyncLoadResInfo>[(int)LoadResPriority.RES_NUM];
    //
    protected Dictionary<uint, AsyncLoadResInfo> m_LoadingAssetDic = new Dictionary<uint, AsyncLoadResInfo>();

    //中间类 回调类 对象池
    protected ClassObjectPool<AsyncLoadResInfo> m_AsyncLoadResParamPool = new ClassObjectPool<AsyncLoadResInfo>(50);

    protected ClassObjectPool<AsyncLoadResCallBackInfo> m_AsyncCallBackPool = new ClassObjectPool<AsyncLoadResCallBackInfo>(100);

    public void Init(MonoBehaviour mono)
    {
        AssetBundleMgr.Instance.LoadAssetBundleConfig();

        for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
        {
            m_LoadingAssetList[i] = new List<AsyncLoadResInfo>();
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
            item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                if (item.m_Obj != null)
                {
                    obj = item.m_Obj as T;
                }
                else
                {
                    obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
                }
                
            }
        }

        CacheResource(path, ref item, crc, obj);
        return obj;

    }

    void CacheResource(string path, ref ResourceItem item, uint crc, Object obj, int addrefcount = 1)
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
        item.m_Obj = obj;
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

    protected void DestroyResourceItem(ResourceItem item, bool destroyCache = false)
    {
        if (item == null || item.RefCount > 0)
        {
            return;
        }

        if (!destroyCache)
        {
            //m_NoRefrenceAssetMapList.InsertToHead(item);
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

        if (item.m_Obj != null)
        {
            item.m_Obj = null;
#if UNITY_EDITOR
            Resources.UnloadUnusedAssets();
#endif
        }
    }


    public bool ReleaseResource(Object obj, bool destroyObj = false)
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
        DestroyResourceItem(item,destroyObj);
        return true;
    }
    //不需要实例化的资源卸载 根据路径
    public bool ReleaseResource(string path,bool destroyObj=false)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        uint crc = Crc32.GetCrc32(path);
        ResourceItem item = null;
        if (!AssetDic.TryGetValue(crc, out item) || null == item)
        {
            Debug.LogError("AssetDic里不存在该资源" +path + "可能释放了多次");
        }
        
        item.Release();
        DestroyResourceItem(item, destroyObj);
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
                DestroyResourceItem(item, true);
            }
        }
    }
    //异步加载协程
    IEnumerator AsyncLoadCor()
    {
        //回调队列
        List<AsyncLoadResCallBackInfo> callBackList = null;
      
        long lastYieldTime = System.DateTime.Now.Ticks;
        while (true)
        {
            bool haveYield = false;
            //遍历需加载资源队列
            for (int i = 0; i < (int)LoadResPriority.RES_NUM; i++)
            {
                List<AsyncLoadResInfo> loadingList = m_LoadingAssetList[i];
                if (loadingList.Count <= 0)
                {
                    continue;
                }

                AsyncLoadResInfo loadingItem = loadingList[0];
                loadingList.RemoveAt(0);
                //取出回调
                callBackList = loadingItem.m_CallBackList;

                Object obj = null;
                ResourceItem item = null;
#if UNITY_EDITOR
                if (!m_loadFromAssetBundle)
                {
                    obj = LoadAssetByEditor<Object>(loadingItem.m_Path);
                    //模拟异步加载
                    yield return new WaitForSeconds(0.5f);
                    item = AssetBundleMgr.Instance.GetResourceItem(loadingItem.m_Crc);
                }
#endif
                if (obj == null)
                {
                    item = AssetBundleMgr.Instance.LoadResourceAssetBundle(loadingItem.m_Crc);
                    if (item != null && item.m_AssetBundle != null)
                    {
                        AssetBundleRequest abRequest = null;
                        if (loadingItem.isSprite)
                        {
                            abRequest = item.m_AssetBundle.LoadAssetAsync<Sprite>(item.m_AssetName);
                        }
                        else
                        {
                            abRequest=item.m_AssetBundle.LoadAssetAsync(item.m_AssetName);
                        }
                            
                        yield return abRequest;
                        if (abRequest.isDone)
                        {
                            obj = abRequest.asset;
                        }

                        lastYieldTime = System.DateTime.Now.Ticks;
                    }
                }

                CacheResource(loadingItem.m_Path, ref item, loadingItem.m_Crc, obj, callBackList.Count);
                for (int j = 0; j < callBackList.Count; j++)
                {
                    AsyncLoadResCallBackInfo callBack = callBackList[j];
                    if (callBack != null && callBack.m_DealFinish != null)
                    {
                        callBack.m_DealFinish(loadingItem.m_Path, obj, callBack.m_Param1, callBack.m_Param2,
                            callBack.m_Param3);//执行回调
                        callBack.m_DealFinish = null;
                    }

                    callBack.Reset();
                    m_AsyncCallBackPool.Recycle(callBack);

                }


                obj = null;
                callBackList.Clear();
                m_LoadingAssetDic.Remove(loadingItem.m_Crc);
                loadingItem.Reset();
                m_AsyncLoadResParamPool.Recycle(loadingItem);

                if (System.DateTime.Now.Ticks - lastYieldTime > MAXLOADRESTIME)//大于这个时间等待一帧
                {
                    yield return null;
                    lastYieldTime = System.DateTime.Now.Ticks;
                    haveYield = true;
                }
            }

            if (!haveYield||System.DateTime.Now.Ticks - lastYieldTime > MAXLOADRESTIME)//大于这个时间等待一帧
            {
                lastYieldTime = System.DateTime.Now.Ticks;
            }

            yield return null;
        }
    }

    public void AsyncLoadResource(string path, OnLoadResFinish dealFinish, LoadResPriority priority,
        object param1 = null, object param2 = null, object param3 = null, uint crc = 0)
    {
        if (crc == 0)
        {
            crc = Crc32.GetCrc32(path);
        }

        ResourceItem item = GetCacheResourceItem(crc);
        if (item != null)
        {
            if (dealFinish != null)
            {
                dealFinish(path, item.m_Obj, param1, param2, param3);
            }

            return;
        }
        //判断是否在加载中
        AsyncLoadResInfo para = null;
        if (!m_LoadingAssetDic.TryGetValue(crc, out para) || para == null)
        {
            para = m_AsyncLoadResParamPool.Spawn();
            para.m_Crc = crc;
            para.m_Path = path;
            para.m_Priority = priority;
            m_LoadingAssetDic.Add(crc, para);
            m_LoadingAssetList[(int)priority].Add(para);
        }

        //往回调列表里面加回调
        AsyncLoadResCallBackInfo callBack = m_AsyncCallBackPool.Spawn();
        callBack.m_DealFinish = dealFinish;
        callBack.m_Param1 = param1;
        callBack.m_Param2 = param2;
        callBack.m_Param3 = param3;
        para.m_CallBackList.Add(callBack);
    }

    public void ClearCache()
    {
        List<ResourceItem> tempList = new List<ResourceItem>();//为了不在foreach中做移除
        foreach (var item in AssetDic.Values)
        {
            if (item.m_Clear)
            {
                tempList.Add(item);
            }
            
        }

        foreach (var item in tempList)
        {
            DestroyResourceItem(item, true);
        }

        tempList.Clear();
//         while (m_NoRefrenceAssetMapList.Size() > 0)
//         {
//             ResourceItem item = m_NoRefrenceAssetMapList.Back();
//             DestroyResourceItem(item, item.m_Clear);
//             m_NoRefrenceAssetMapList.Pop();
//         }
    }
    //预加载，跳场景不清除的资源
    public void PreLoad(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        uint crc = Crc32.GetCrc32(path);
        ResourceItem item = GetCacheResourceItem(crc,0);//预加载 不添加引用
        if (item != null)
        {
            return;
        }

        Object obj = null;
#if UNITY_EDITOR
        if (!m_loadFromAssetBundle)//编辑器加载
        {
            obj = LoadAssetByEditor<Object>(path);
            item = AssetBundleMgr.Instance.GetResourceItem(crc);
        }
#endif
        if (obj == null)
        {
            item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
            if (item != null && item.m_AssetBundle != null)
            {
                obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
            }
        }

        CacheResource(path, ref item, crc, obj);
        //设置跳场景 不清空
        item.m_Clear = false;
        ReleaseResource(obj, false);
    }

    public ResourceObj LoadResoruce(string path,ResourceObj resObj)
    {
        if (resObj == null)
        {
            return null;
        }

        uint crc = resObj.m_Crc == 0 ? Crc32.GetCrc32(path) : resObj.m_Crc;
        ResourceItem item = GetCacheResourceItem(crc);
        if (item != null)
        {
            resObj.m_ResItem = item;
            return resObj;
        }

        Object obj = null;

#if UNITY_EDITOR
        if (!m_loadFromAssetBundle)
        {
            obj = LoadAssetByEditor<Object>(path);
            item = AssetBundleMgr.Instance.GetResourceItem(resObj.m_Crc);
            if (item.m_Obj != null)
            {
                obj = item.m_Obj as Object;
            }
            else
            {
                obj = LoadAssetByEditor<Object>(path);
            }
        }
#endif
        item = AssetBundleMgr.Instance.LoadResourceAssetBundle(crc);
        if (item != null && item.m_AssetBundle != null)
        {
            if (item.m_Obj != null)
            {
                obj = item.m_Obj as Object;
            }
            else
            {
                obj = item.m_AssetBundle.LoadAsset<Object>(item.m_AssetName);
            }

        }

        CacheResource(path, ref item, crc, obj);
        resObj.m_ResItem = item;
        item.m_Clear = resObj.m_bClear;

        return resObj;
    }

    

}


