
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
    [Serializable]
    public class UIPanelDataMgr: ScriptableObject
    {
        public List<UIPanelInfo> PanelInfoList;
    }
}
