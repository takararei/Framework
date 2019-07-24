//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//namespace Assets.Framework.Tools
//{
//    public static class DGTool
//    {
//        public static void DoScaleShow(Transform transform)
//        {
//            transform.localScale = Vector3.zero;
//            transform.gameObject.SetActive(true);
//            transform.DOScale(1, 0.5f);
//        }

//        public static void DoScaleHide(Transform transform)
//        {
//            transform.DOScale(0, 0.5f).onComplete = 
//                () => transform.gameObject.SetActive(false);
//            //transform.gameObject.SetActive(false);
//        }

//        public static void DoFadeShow(CanvasGroup canvasGroup)
//        {
//            canvasGroup.alpha = 0;
//            canvasGroup.gameObject.SetActive(true);
//            canvasGroup.DOFade(1, 0.5f);
//        }

//        public static void DoFadeHide(CanvasGroup canvasGroup)
//        {
//            canvasGroup.DOFade(0, 0.5f).onComplete = 
//                () => canvasGroup.gameObject.SetActive(false);
//            //canvasGroup.gameObject.SetActive(false);
//        }

//    }
//}
