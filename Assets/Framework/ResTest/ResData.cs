using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    
}
