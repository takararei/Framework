using UnityEngine;

namespace Assets.Framework.UI
{
    public enum UILayer
    {
        Common,
        Bottom,
        Top,
    }
    public static class UIBusiness
    {
        public static UIPanelInfo GetPanelInfo(UIPanelName name)
        {
            int index = (int)name;
            if(index>=panelInfoArr.Length)
            {
                Debug.LogError("没有该面板的PanelInfo:" + name.ToString());
                throw new System.Exception();
            }
            return panelInfoArr[(int)name];
        }

        private static UIPanelInfo[] panelInfoArr = new UIPanelInfo[]
        {
            new UIPanelInfo(){Name=UIPanelName.GameStartPanel,Layer=UILayer.Common},
        };
        public static BasePanel GetPanelBusiness(UIPanelName name)
        {
            switch (name)
            {
                case UIPanelName.GameStartPanel: return new GameStartPanel();


                default:
                    Debug.LogError("不存在此面板" + name.ToString());
                    return null;

            }

        }

    }
    public enum UIPanelName:int
    {
        GameStartPanel = 0,
    }

}
