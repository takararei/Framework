using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//资源记录
public class ResourceItem
{
    /// <summary>
    /// 地址crc
    /// </summary>
    public uint m_Crc = 0;
    /// <summary>
    /// 资源名称
    /// </summary>
    public string m_AssetName = string.Empty;
    /// <summary>
    /// 资源所在的AB包名称
    /// </summary>
    public string m_ABName = string.Empty;
    /// <summary>
    /// 资源所在AB包的依赖包名称
    /// </summary>
    public List<string> m_DependAssetBundle = null;
    /// <summary>
    /// 资源所在的AB包
    /// </summary>
    public AssetBundle m_AssetBundle = null;
    /// <summary>
    /// 资源本体
    /// </summary>
    public Object m_Obj = null;
    /// <summary>
    /// 资源唯一标识 instanceID
    /// </summary>
    public int m_Guid = 0;
    //资源最后所使用的时间
    public float m_LastUseTime = 0.0f;
    //引用计数
    protected int m_RefCount = 0;
    //是否跳场景清掉
    public bool m_Clear = true;
    public int RefCount
    {
        get { return m_RefCount; }
        set
        {
            m_RefCount = value;
            if (m_RefCount < 0)
            {
                Debug.LogError("refcount < 0" + m_RefCount + " ," + (m_Obj != null ? m_Obj.name : "name is null"));
            }
        }
    }

    public void Release(int refCount=1)
    {
        m_RefCount-=refCount;
        if (m_RefCount <= 0)
        {
            OnZeroRelease();
        }

    }

    public void Retain(int refCount=1)
    {
        m_RefCount += refCount;
    }

    public virtual void OnZeroRelease()
    {

    }
}