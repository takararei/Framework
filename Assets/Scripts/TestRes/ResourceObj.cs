using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ResourceObj
{
    public uint m_Crc = 0;
    public ResourceItem m_ResItem;
    //实例化的物体
    public GameObject m_CloneObj = null;
    public bool m_bClear = true;
    public long m_Guid = 0;
    public bool m_Already = false;
    public void Reset()
    {
        m_Crc = 0;
        m_CloneObj = null;
        m_bClear = true;
        m_Guid = 0;
        m_Already = false;
    }
}
