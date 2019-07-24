
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Framework.Res
{
    public class BaseRes:IBaseRes
    {
        protected Dictionary<string, Object> factoryDict = new Dictionary<string, Object>();

        protected string LoadPath;
        public T GetRes<T>(string resourcePath)where T:Object
        {
            Object item = null;
            if(!factoryDict.TryGetValue(resourcePath,out item))
            {
                item = Resources.Load<T>(resourcePath);
                if (item == null)
                {
                    Debug.Log(resourcePath + "获取失败，路径有误");
                }
                else
                {
                    factoryDict.Add(resourcePath, item);
                }
            }

            return item as T;
        }
    }
}
