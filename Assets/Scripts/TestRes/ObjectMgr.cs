using System.Collections;
using System.Collections.Generic;
using Assets.Framework.Singleton;
using System;
using UnityEngine;

public class ObjectMgr : Singleton<ObjectMgr>
{
    //类对象池 字典
    protected Dictionary<Type, object> m_ClassPoolDict = new Dictionary<Type, object>();

    //创建类对象池
    public ClassObjectPool<T> GetOrCreateClassPool<T>(int maxCount) where T : class, new()
    {
        Type type = typeof(T);
        object outObj = null;
        if (!m_ClassPoolDict.TryGetValue(type, out outObj) || outObj == null)
        {
            ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxCount);
            m_ClassPoolDict.Add(type, newPool);
            return newPool;
        }

        return outObj as ClassObjectPool<T>;
    }
    //对象池节点
    public Transform RecyclePoolTrs;
    public Transform SceneTrs;
    protected Dictionary<uint, List<ResourceObj>> m_ObjectPoolDict = new Dictionary<uint, List<ResourceObj>>();
    //存每一种prefab的对象池
    protected ClassObjectPool<ResourceObj> m_ResourceObjClassPool =
        ObjectMgr.Instance.GetOrCreateClassPool<ResourceObj>(1000);
    //存每个示例出来的物体 key为guid
    protected Dictionary<int, ResourceObj> m_ResourceObjDic = new Dictionary<int, ResourceObj>();


    public override void Init()
    {
        base.Init();
        RecyclePoolTrs = GameObject.Find("RecyclePool").transform;
        RecyclePoolTrs.gameObject.SetActive(false);
        SceneTrs = GameObject.Find("SceneTrans").transform;
    }

    protected ResourceObj GetObjectFromPool(uint crc)
    {
        List<ResourceObj> st = null;
        if (m_ObjectPoolDict.TryGetValue(crc, out st) && st != null && st.Count > 0)
        {
            ResourceObj resObj = st[0];
            st.RemoveAt(0);
            GameObject obj = resObj.m_CloneObj;
            if (!System.Object.ReferenceEquals(obj, null))//这个比==快
            {
#if UNITY_EDITOR
                if (obj.name.EndsWith("(Recycle)"))
                {
                    obj.name = obj.name.Replace("(Recycle)", "");
                }
    
#endif
            }

            return resObj;
        }

        return null;
    }

    public GameObject InstantiateObject(string path,bool setSceneObj=false,bool bClear=true)//是否跳场景清除
    {
        uint crc = Crc32.GetCrc32(path);
        ResourceObj resourceObj=GetObjectFromPool(crc);
        if (resourceObj == null)
        {
            resourceObj = m_ResourceObjClassPool.Spawn();
            resourceObj.m_Crc = crc;
            resourceObj.m_bClear = bClear;
            resourceObj.m_ResItem = null;
            resourceObj = ResourceMgr.Instance.LoadResoruce(path, resourceObj);
            if (resourceObj.m_ResItem.m_Obj != null)
            {
                resourceObj.m_CloneObj = GameObject.Instantiate(resourceObj.m_ResItem.m_Obj) as GameObject;
            }
        }

        if (setSceneObj)
        {
            resourceObj.m_CloneObj.transform.SetParent(SceneTrs,false);
        }

        int tempID = resourceObj.m_CloneObj.GetInstanceID();
        if (!m_ResourceObjDic.ContainsKey(tempID))
        {
            m_ResourceObjDic.Add(tempID, resourceObj);
        }

        return resourceObj.m_CloneObj;
        
    }

    public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destroyCache = false,
        bool recycleParent = true)
    {
        if (obj == null)
        {
            return;
        }

        ResourceObj resObj = null;
        int tempID = obj.GetInstanceID();
        if (!m_ResourceObjDic.TryGetValue(tempID,out resObj))
        {
            Debug.LogError("对象不是ObjMgr创建的");
            return;
        }

        if (resObj == null)
        {
            Debug.LogError("缓存的resObj为空");
        }


        if (resObj.m_Already)
        {
            Debug.LogError("对象已被放回对象池，是否清除引用");
            return;
        }
    }

}
