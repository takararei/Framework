using System.Collections;
using System.Collections.Generic;
using Assets.Framework.Singleton;
using UnityEngine;

public class ResourceMgr:Singleton<ResourceMgr>
{
    //缓存引用计数为零的资源列表，达到缓存最大的时候释放这个列表里面最早没用的资源
    protected CMapList<ResourceItem> m_NoRefrenceAssetMapList = new CMapList<ResourceItem>();
}


