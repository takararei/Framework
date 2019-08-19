using System;
using System.Collections.Generic;
using UnityEngine;
using Assets.Framework.Util;
using Object = UnityEngine.Object;

namespace Assets.Framework
{
    [Serializable]
    public class ResInfo
    {
        public string name;
        public string path;
        public string abName;

    }
    [Serializable]
    public class ABInfo
    {
        public string abName;
        public string path;
        public List<string> dependencies;
        public List<ResInfo> resList;
    }


    public class ResItem:SimpleRC
    {
        public uint Crc;
        public string AssetName;
        public string ABName;
        public List<string> DependentAB;
        public AssetBundle AB;
        public Object Asset;
        public int Guid;

        public float LastUseTime = 0;

        public void Reset()
        {

        }

        protected override void OnZeroRef()
        {
            base.OnZeroRef();
        }

    }

    public class ABItem: SimpleRC
    {
        public AssetBundle AB;

        public void Reset()
        {
            AB = null;
        }

        protected override void OnZeroRef()
        {
            base.OnZeroRef();
            if (AB != null)
            {
                AB.Unload(true);
                Reset();
            }
        }

    }




}
