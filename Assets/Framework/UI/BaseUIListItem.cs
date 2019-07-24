using Assets.Framework.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.UI
{
    public class BaseUIListItem:IBaseUIListItem
    {
        public int id
        {
            get;
            set;
        }

        public GameObject root
        {
            get;
            set;
        }

        protected T Find<T>(string uiName)
        {
            return UITool.FindChild<T>(root, uiName);
        }

        public virtual void Clear()
        {

        }
        
    }
}
