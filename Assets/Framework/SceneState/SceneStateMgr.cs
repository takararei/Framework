
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
    public class SceneStateMgr:Singleton<SceneStateMgr>
    {

        public IBaseSceneState lastSceneState;
        public IBaseSceneState currentSceneState;

        //遮罩相关
        private GameObject mask;
        private Image maskImage;
        private float maskTime = 1.5f;

        public override void Init()
        {
            base.Init();
            //InitMask();
            //currentSceneState = new StartLoadSceneState();
            //currentSceneState.EnterScene();
            //if (GameRoot.Instance.toMainScene)
            //{
            //    SceneManager.LoadScene(2);
            //    currentSceneState = new MainSceneState();
            //    currentSceneState.EnterScene();
            //}else
            //{
            //    currentSceneState = new StartLoadSceneState();
            //    currentSceneState.EnterScene();
            //}
        }

        //private void InitMask()
        //{
        //    mask = UIMgr.Instance.uiFacade.CreateUIAndSetUIPosition("Img_Mask");
        //    maskImage = mask.GetComponent<Image>();
        //}

        private void ShowMask()
        {
            //mask.transform.SetSiblingIndex(10);
            //Tween t =
            //    DOTween.To(() => maskImage.color,
            //    toColor => maskImage.color = toColor,
            //    new Color(0, 0, 0, 1),
            //    maskTime);
            //t.OnComplete(ExitSceneComplete);
            ExitSceneComplete();
        }

        //private void HideMask()
        //{
        //    DOTween.To(() => maskImage.color,
        //        toColor => maskImage.color = toColor,
        //        new Color(0, 0, 0, 0),
        //        maskTime);
            
        //}

        public void ChangeSceneState(IBaseSceneState baseSceneState)
        {
            lastSceneState = currentSceneState;
            ShowMask();
            currentSceneState = baseSceneState;
        }

        private void ExitSceneComplete()
        {
            lastSceneState.ExitScene();
            currentSceneState.EnterScene();
            //HideMask();
        }

    }
}
