﻿using Assets.Framework.Extension;
using Assets.Framework.Res;
//using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Framework.UI
{
    public class UIFacade
    {
        private const string UIRootName = "Canvas";
        private readonly string uiPrefabPath = "Prefabs/UIPanels/";
        /// <summary>
        /// 面板实例的位置
        /// </summary>
        #region Transform
        private Transform _canvasTransform;
        private Transform _bgTransform;
        private Transform _commonTransform;
        private Transform _topTransform;
        public Transform CanvasTransform
        {
            get
            {
                if (_canvasTransform == null)
                {
                    _canvasTransform = GameObject.Find(UIRootName).transform;
                }
                return _canvasTransform;
            }
        }

        public Transform BGTransform
        {
            get
            {
                if (_bgTransform == null)
                {
                    _bgTransform = CanvasTransform.Find(UILayer.Bottom.ToString());
                }
                return _bgTransform;
            }
        }

        public Transform CommonTransform
        {

            get
            {
                if (_commonTransform == null)
                {
                    _commonTransform = CanvasTransform.Find(UILayer.Common.ToString());
                }
                return _commonTransform;

            }
        }

        public Transform TopTransform
        {
            get
            {
                if (_topTransform == null)
                {
                    _topTransform = CanvasTransform.Find(UILayer.Top.ToString());
                }
                return _topTransform;
            }
        }
        #endregion Transform
        

        #region Panel-Dict
        /// <summary>
        /// 保存所有被实例化的BasePanel组件,BasePanel 脚本
        /// </summary>
        private Dictionary<UIPanelName, IBasePanel> panelDict;
        /// <summary>
        /// 管理所有显示的面板 脚本
        /// </summary>
        private Dictionary<UIPanelName, IBasePanel> panelShowDict;
        /// <summary>
        /// 管理实例化的面板游戏物体，预制体
        /// </summary>
        private Dictionary<UIPanelName, GameObject> panelGODict;
        /// <summary>
        /// 当前场景的面板游戏物体，预制体
        /// </summary>
        public Dictionary<UIPanelName, GameObject> currentScenePanelDict;
        
        #endregion

        public UIFacade()
        {
            panelDict = new Dictionary<UIPanelName, IBasePanel>();
            panelShowDict = new Dictionary<UIPanelName, IBasePanel>();
            panelGODict = new Dictionary<UIPanelName, GameObject>();
            currentScenePanelDict = new Dictionary<UIPanelName, GameObject>();
        }

        public void ClearPanelDict()
        {
            //UIManager.Instance.ClearDict();
            foreach (var item in currentScenePanelDict)
            {
                item.Value.transform.SetParent(GameRoot.Instance.transform);
                if(item.Value.activeSelf!=false)
                {
                    UIMgr.Instance.Hide(item.Key);
                }
            }
            currentScenePanelDict.Clear();

            panelShowDict.Clear();
            //panelDict.Clear();//待定
            foreach(var item in panelDict)
            {
                item.Value.OnDestroy();
            }
            panelDict.Clear();
            
        }


        /// <summary>
        /// 获取面板的游戏物体
        /// </summary>
        private GameObject GetPanelGO(UIPanelName panelName,string path)
        {
            GameObject instPanel = panelGODict.TryGet(panelName);
            

            if (instPanel == null)
            {
                GameObject res = ResMgr.Instance.GetRes<GameObject>(uiPrefabPath + panelName.ToString());
                instPanel = GameObject.Instantiate(res) as GameObject;
                instPanel.name = panelName.ToString();
                panelGODict.Add(panelName, instPanel);
            }

            if (!currentScenePanelDict.ContainsKey(panelName))
                currentScenePanelDict.Add(panelName, instPanel);
            return instPanel;
        }

        /// <summary>
        /// 根据面板类型得到实例化面板
        /// </summary>
        private IBasePanel GetPanel(UIPanelName panelName)
        {
            IBasePanel panel = panelDict.TryGet(panelName);

            if (panel == null)
            {
                //如果找不到 就实例
                UIPanelInfo pInfo = UIPanelHelper.GetPanelInfo(panelName);

                GameObject instPanel = GetPanelGO(panelName, pInfo.path);

                switch (pInfo.Layer)
                {
                    case UILayer.Bottom:
                        instPanel.transform.SetParent(BGTransform, false);
                        break;
                    case UILayer.Common:
                        instPanel.transform.SetParent(CommonTransform, false);
                        break;
                    case UILayer.Top:
                        instPanel.transform.SetParent(TopTransform, false);
                        break;
                    default:
                        Debug.LogError(pInfo.Name + "没有设置层级");
                        break;
                }
                instPanel.transform.SetParent(CommonTransform, false);
                instPanel.transform.ResetLocal();
                
                panel = UIPanelHelper.GetPanelBusiness(panelName);
                panel.rootUI = instPanel;

                panelDict.Add(panelName, panel);
                panel.Init();
            }
            return panel;
        }

        public void Show(UIPanelName panelName)
        {
            IBasePanel panel = GetPanel(panelName);
            panel.rootUI.transform.SetAsLastSibling();
            panel.OnShow();

            if (!panelShowDict.ContainsKey(panelName))
                panelShowDict.Add(panelName, panel);
        }

        public void Hide(UIPanelName panelName)
        {
            if (panelShowDict.Count <= 0) return;

            IBasePanel panel = panelShowDict.TryGet(panelName);

            if (panel == null) return;

            panel.OnHide();
            panelShowDict.Remove(panelName);

        }

        public void Update()
        {
            if (panelShowDict.Count == 0) return;
            foreach (KeyValuePair<UIPanelName, IBasePanel> panel in panelShowDict)
            {
                panel.Value.Update();
            }
        }
        
        //UI部分
        //public GameObject CreateUIAndSetUIPosition(string uiName)
        //{
        //    GameObject itemGo = FactoryMgr.Instance.GetUI(uiName);
        //    itemGo.transform.SetParent(CanvasTransform);
        //    itemGo.transform.ResetLocal();
        //    return itemGo;
        //}

    }
}
