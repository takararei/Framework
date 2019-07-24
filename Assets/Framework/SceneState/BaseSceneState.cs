using Assets.Framework.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Framework.SceneState
{
    public class BaseSceneState : IBaseSceneState
    {
        public SceneName scene
        {
            get;

            set;
        }

        public virtual void EnterScene()
        {
            
        }

        public virtual void ExitScene()
        {
            UIMgr.Instance.uiFacade.ClearPanelDict();

        }
    }
}

