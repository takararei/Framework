
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 异步加载资源的信息，含回调信息
/// </summary>
public class AsyncLoadResInfo//用于记录排队等待加载的资源信息 和回调
{
    public List<AsyncLoadResCallBackInfo> m_CallBackList = new List<AsyncLoadResCallBackInfo>();
    public uint m_Crc;
    public string m_Path;
    public LoadResPriority m_Priority = LoadResPriority.RES_SLOW;
    public bool isSprite = false;
    public void Reset()
    {
        m_Crc = 0;
        m_Path = "";
        m_Priority = LoadResPriority.RES_SLOW;
        isSprite = false;

    }
}
/// <summary>
/// 异步加载资源的回调信息
/// </summary>
public class AsyncLoadResCallBackInfo//回调
{
    public OnLoadResFinish m_DealFinish = null;
    public object m_Param1 = null, m_Param2 = null, m_Param3 = null;

    public void Reset()
    {
        m_DealFinish = null;
        m_Param1 = null;
        m_Param2 = null;
        m_Param3 = null;

    }
}
public class ResHelper
{
    private const int MAX_CACHE_COUNT = 500;
    private const long MAX_LOAD_RES_TIME = 200000;
    //对象池
    protected ClassObjectPool<AsyncLoadResInfo> m_AsyncLoadResParamPool = new ClassObjectPool<AsyncLoadResInfo>(50);
    protected ClassObjectPool<AsyncLoadResCallBackInfo> m_AsyncCallBackPool = new ClassObjectPool<AsyncLoadResCallBackInfo>(100);

    public bool m_loadFromAssetBundle = true;
    /// <summary>
    /// 缓存引用计数为零的资源列表，达到缓存最大的时候释放这个列表里面最早没用的资源
    /// </summary>
    protected CMapList<ResourceItem> m_NoRefrenceAssetMapList = new CMapList<ResourceItem>();
    /// <summary>
    /// 记录一下正在使用的ResourceItem
    /// </summary>
    protected Dictionary<uint, ResourceItem> AssetDic { get; set; } = new Dictionary<uint, ResourceItem>();

    //Mono脚本
    protected MonoBehaviour m_Startmono;
    protected List<AsyncLoadResInfo>[] m_LoadingAssetList;
    protected Dictionary<uint, AsyncLoadResInfo> m_LoadingAssetDic;

    public ResHelper(MonoBehaviour mono)
    {
        m_LoadingAssetList = new List<AsyncLoadResInfo>[(int)LoadResPriority.RES_NUM];
        m_LoadingAssetDic = new Dictionary<uint, AsyncLoadResInfo>();
        for (var i = 0; i < (int)LoadResPriority.RES_NUM; i++)
        {
            m_LoadingAssetList[i] = new List<AsyncLoadResInfo>();
        }
        m_Startmono = mono;
        m_Startmono.StartCoroutine(AsyncLoadCor());
    }

    public T LoadResource<T>(string path) where T : Object
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var crc = Crc32.GetCrc32(path);
        var item = GetCacheResourceItem(crc);
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
                obj = item.m_AssetBundle.LoadAsset<T>(item.m_AssetName);
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
            return;
        }

        if (obj == null)
        {
            Debug.LogError("ResourceLoad Fail " + path);
            return;
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
        if (AssetDic.TryGetValue(crc, out item) && item != null)
        {
            item.Retain(addrefcount);
            item.m_LastUseTime = Time.realtimeSinceStartup;
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
        DestroyResourceItem(item, destroyObj);
        return true;
    }

    /// <summary>
    /// 缓存太多，清除最早没有使用的资源
    /// </summary>
    protected void WashOut()
    {
        //当大于缓存个数时，进行一半释放
        while (m_NoRefrenceAssetMapList.Size() >= MAX_CACHE_COUNT)
        {
            for (int i = 0; i < MAX_CACHE_COUNT / 2; i++)
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

        var lastYieldTime = System.DateTime.Now.Ticks;
        while (true)
        {
            var haveYield = false;
            //遍历需加载资源队列
            for (var i = 0; i < (int)LoadResPriority.RES_NUM; i++)//加载每个级别的第一个元素
            {
                var loadingList = m_LoadingAssetList[i];
                if (loadingList.Count <= 0)
                {
                    continue;
                }

                var loadingItem = loadingList[0];
                loadingList.RemoveAt(0);
                //取出回调
                var callBackList = loadingItem.m_CallBackList;

                Object obj = null;
                ResourceItem item = null;
#if UNITY_EDITOR
                if (!m_loadFromAssetBundle)//Editor的异步加载
                {
                    obj = LoadAssetByEditor<Object>(loadingItem.m_Path);
                    //模拟异步加载
                    yield return new WaitForSeconds(0.5f);
                    item = AssetBundleMgr.Instance.GetResourceItem(loadingItem.m_Crc);
                }
#endif
                //从AB包加载资源出来
                if (obj == null)
                {
                    item = AssetBundleMgr.Instance.LoadResourceAssetBundle(loadingItem.m_Crc);
                    if (item != null && item.m_AssetBundle != null)
                    {
                        AssetBundleRequest abRequest = null;
                        abRequest = loadingItem.isSprite ? item.m_AssetBundle.LoadAssetAsync<Sprite>(item.m_AssetName) : item.m_AssetBundle.LoadAssetAsync(item.m_AssetName);

                        yield return abRequest;
                        if (abRequest.isDone)
                        {
                            obj = abRequest.asset;
                        }

                        lastYieldTime = System.DateTime.Now.Ticks;
                    }
                }

                CacheResource(loadingItem.m_Path, ref item, loadingItem.m_Crc, obj, callBackList.Count);
                foreach (var callBack in callBackList)
                {
                    if (callBack?.m_DealFinish != null)
                    {
                        callBack.m_DealFinish(loadingItem.m_Path, obj, callBack.m_Param1, callBack.m_Param2,
                            callBack.m_Param3);//执行回调
                        callBack.Reset();
                    }

                    m_AsyncCallBackPool.Recycle(callBack);
                }


                obj = null;
                callBackList.Clear();
                m_LoadingAssetDic.Remove(loadingItem.m_Crc);
                loadingItem.Reset();
                m_AsyncLoadResParamPool.Recycle(loadingItem);

                if (System.DateTime.Now.Ticks - lastYieldTime > MAX_LOAD_RES_TIME)//大于这个时间等待一帧
                {
                    yield return null;
                    lastYieldTime = System.DateTime.Now.Ticks;
                    haveYield = true;
                }
            }

            if (!haveYield || System.DateTime.Now.Ticks - lastYieldTime > MAX_LOAD_RES_TIME)//大于这个时间等待一帧
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

        var item = GetCacheResourceItem(crc);
        if (item != null)
        {
            dealFinish?.Invoke(path, item.m_Obj, param1, param2, param3);

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
        var callBack = m_AsyncCallBackPool.Spawn();
        callBack.m_DealFinish = dealFinish;
        callBack.m_Param1 = param1;
        callBack.m_Param2 = param2;
        callBack.m_Param3 = param3;
        para.m_CallBackList.Add(callBack);
    }

}

