using UnityEngine;

namespace Assets.Framework.UI
{
    public static class UIBusiness
    {
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
    public enum UIPanelName
    {
        GameStartPanel = 0,
    }

}
