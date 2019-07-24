using Assets.Framework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
    public class UILayer
    {
        public const string Background = "Background";
        public const string Common = "Common";
        public const string Top = "Top";
    }
    public class BasePanel : IBasePanel
    {
        public GameObject rootUI
        {
            get;
            set;
        }

        protected CanvasGroup canvasGroup;
        protected GameObject UICanvas;
        public virtual void Init()
        {
            //UICanvas = UIManager.GetInstance().CanvasTransform.gameObject;
        }

        protected bool SetCanvasGroup()
        {
            canvasGroup = rootUI.transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.Log(rootUI.name + "没有canvasGroup组件");
                return false;
            }
            //return canvasGroup;
            return true;
        }


        public virtual void Update()
        {

        }


        public virtual void OnShow()
        {
            if (rootUI.activeSelf == true) return;
            rootUI.SetActive(true);
        }

        //public virtual void OnPause()
        //{
        //}

        //public virtual void OnResume()
        //{
        //}

        public virtual void OnHide()
        {
            if (rootUI.activeSelf == false) return;
            rootUI.SetActive(false);
        }

        protected T Find<T>(string uiName)
        {
            return UITool.FindChild<T>(rootUI, uiName);
        }

        public virtual void OnDestroy()
        {

        }
    }
}
