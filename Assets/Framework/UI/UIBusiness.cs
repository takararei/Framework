using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
    public static class UIBusiness
    {
        public static BasePanel GetPanelBusiness(UIPanelName name)
        {
            switch (name)
            {
                //case UIPanelName.TestUIPanel:return new TestUIPanel();
                default:
                    Debug.LogError("不存在此面板" + name.ToString());
                    return null;

            }

        }
    }

    public enum UIPanelName
    {
        TestUIPanel = 0,
    }
}
