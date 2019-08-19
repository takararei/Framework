using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Framework
{
    [Serializable]
    public class MyABConfig : ScriptableObject
    {
        public List<ABInfo> List;
    }
}
