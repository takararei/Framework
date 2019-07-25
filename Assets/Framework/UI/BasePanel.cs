using Assets.Framework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
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

        }

        public virtual void Update()
        {

        }


        public virtual void OnShow()
        {
            if (rootUI.activeSelf) return;
            rootUI.SetActive(true);
        }

        public virtual void OnHide()
        {
            if (!rootUI.activeSelf) return;
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
