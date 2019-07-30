using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Framework.Singleton;
using Assets.Framework.Util;
using UnityEngine;

public class AssetBundleMgr : Singleton<AssetBundleMgr>
{
    //记录加载的资源块信息 事先加载好了所有的资源块信息
    protected Dictionary<uint, ResourceItem> m_ResourceItemDic = new Dictionary<uint, ResourceItem>();
    /// <summary>
    /// 记录已经加载的AB包
    /// </summary>
    protected Dictionary<uint, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
    //AB包信息的资源池
    protected ClassObjectPool<AssetBundleItem> m_AssetBundleItemPool = ObjectMgr.Instance.GetOrCreateClassPool<AssetBundleItem>(500);

    protected string ABLoadPath
    {
        get
        {
            return Application.streamingAssetsPath + "/";
        }
    }

    /// <summary>
    /// 加载配置表
    /// </summary>
    /// <returns></returns>
    public bool LoadAssetBundleConfig()
    {
        m_ResourceItemDic.Clear();
        //加载AB
        string configPath = Application.streamingAssetsPath + "/assetbundleconfig";
        AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
        TextAsset textAsset = configAB.LoadAsset<TextAsset>("assetbundleconfig");
        if (textAsset == null)
        {
            Debug.LogError("AsserBundleConfig is not exist");
        }
        //反序列化
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();

        for (int i = 0; i < config.ABList.Count; i++)
        {
            ABBase abBase = config.ABList[i];
            ResourceItem item = new ResourceItem();
            item.m_Crc = abBase.Crc;
            item.m_AssetName = abBase.AssetName;
            item.m_ABName = abBase.ABName;
            item.m_DependAssetBundle = abBase.ABDependence;
            if (m_ResourceItemDic.ContainsKey(item.m_Crc))
            {
                Debug.LogError("重复的Crc 资源名:" + item.m_AssetName + " ab包名：" + item.m_ABName);
            }
            else
            {
                m_ResourceItemDic.Add(item.m_Crc, item);
            }
        }
        return true;
    }
    //加载包里的资源信息
    public ResourceItem LoadResouceAssetBundle(uint crc)
    {
        ResourceItem item = GetResourceItem(crc);

        //if (!m_ResourceItemDic.TryGetValue(crc, out item) || item == null)
        //{
        //    Debug.LogError(string.Format("LoadResourceAssetBundle error: can not find crc {0} in AssetBundleConfig", crc.ToString()));
        //    return item;
        //}

        if (item == null)
        {
            Debug.LogError(string.Format("LoadResourceAssetBundle error: can not find crc {0} in AssetBundleConfig", crc.ToString()));
            return item;
        }

        if (item.m_AssetBundle != null)
        {
            return item;
        }

        item.m_AssetBundle = LoadAssetBundle(item.m_ABName);

        if (item.m_DependAssetBundle != null)
        {
            for (int i = 0; i < item.m_DependAssetBundle.Count; i++)
            {
                LoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }

        return item;
    }
    public ResourceItem GetResourceItem(uint crc)
    {
        ResourceItem item = null;
        m_ResourceItemDic.TryGetValue(crc, out item);
        return item;
    }
    public void ReleaseAsset(ResourceItem item)
    {
        if (item == null)
        {
            return;
        }

        if (item.m_DependAssetBundle != null && item.m_DependAssetBundle.Count > 0)
        {
            for (int i = 0; i < item.m_DependAssetBundle.Count; i++)
            {
                UnLoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }
        UnLoadAssetBundle(item.m_ABName);
    }

    private AssetBundle LoadAssetBundle(string name)
    {
        AssetBundleItem item = null;
        uint crc = Crc32.GetCrc32(name);

        if (!m_AssetBundleItemDic.TryGetValue(crc, out item))
        {
            AssetBundle assetBundle = null;
            string fullPath = ABLoadPath + name;
            assetBundle = AssetBundle.LoadFromFile(fullPath);

            if (assetBundle == null)
            {
                Debug.LogError(" Load AssetBundle Error:" + fullPath);
            }

            item = m_AssetBundleItemPool.Spawn();
            item.assetBundle = assetBundle;
            item.Retain();
            m_AssetBundleItemDic.Add(crc, item);
        }
        else
        {
            item.Retain();
        }
        return item.assetBundle;
    }
    private void UnLoadAssetBundle(string name)
    {
        AssetBundleItem item = null;
        uint crc = Crc32.GetCrc32(name);
        if (m_AssetBundleItemDic.TryGetValue(crc, out item) && item != null)
        {
            item.Release();
            if (item.RefCount <= 0 && item.assetBundle != null)
            {
                item.assetBundle.Unload(true);
                item.Reset();
                m_AssetBundleItemPool.Recycle(item);
                m_AssetBundleItemDic.Remove(crc);
            }
        }
    }

    
}

