using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
    public interface IBasePanel
    {
        GameObject rootUI { get; set; }
        /// <summary>
        /// 处理一些初始化,如按钮赋值 注册监听
        /// </summary>
        void Init();

        /// <summary>
        /// 界面显示
        /// </summary>
        void OnShow();

        /// <summary>
        /// 界面隐藏
        /// </summary>
        void OnHide();

        /// <summary>
        /// 更新
        /// </summary>
        void Update();
        
        /// <summary>
        /// 清理
        /// </summary>
        void OnDestroy();
    }
}
