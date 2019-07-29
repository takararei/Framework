using UnityEngine;
using System.Collections;

//包
public class AssetBundleItem
{
    public AssetBundle assetBundle = null;
    public int RefCount;

    public void Reset()
    {
        assetBundle = null;
        RefCount = 0;
    }

    public void Retain(int refCount=1)
    {
        RefCount += refCount;
    }

    public void Release(int refCount=1)
    {
        RefCount -= refCount;
        if (RefCount <= 0)
        {
            OnZeroRelease();
        }
    }

    public virtual void OnZeroRelease()
    {
        //if (assetBundle != null)
        //{
        //    assetBundle.Unload(true);
        //}
        //Reset();
    }
}