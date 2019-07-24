
using Assets.Framework.Singleton;
using Assets.Framework.UI;
//using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Framework.SceneState
{
    public class SceneStateMgr : Singleton<SceneStateMgr>
    {
        public IBaseSceneState lastSceneState;
        public IBaseSceneState currentSceneState;
        
        public override void Init()
        {
            base.Init();
       
        }
        
        public void GoToScene(SceneName name)
        {
            lastSceneState = currentSceneState;
            currentSceneState = SceneType.GetScene(name);
            currentSceneState.scene = name;
            ExitSceneComplete();
        }


        private void ExitSceneComplete()
        {
            //进加载
            if (lastSceneState != null)
            {
                lastSceneState.ExitScene();
                SceneManager.LoadScene((int)currentSceneState.scene);
            }
            currentSceneState.EnterScene();
            //进正式场景
      
        }

    }
}
