using Assets.Framework.Extension;
using Assets.Framework.Res;
using Assets.Framework.SceneState;
using Assets.Framework.Singleton;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Framework.UI
{
    public class UIMgr : Singleton<UIMgr>
    {
        private UIFacade uiFacade;
        
        public override void Init()
        {
            base.Init();
            uiFacade = new UIFacade();
        }
        
        

        public void Show(UIPanelName panelName)
        {
            uiFacade.Show(panelName);
        }

        public void Hide(UIPanelName panelName)
        {
            uiFacade.Hide(panelName);
        }

        public void Update()
        {
            uiFacade.Update();
        }

        public void ClearPanelDict()
        {
            uiFacade.ClearPanelDict();
        }


    }
}
