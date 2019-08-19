
using Assets.Framework.Tools;
using UnityEngine;
//namespace Assets.Framework.Extension
//{
    public static class GameObjectExtension
    {
        public static void Show(this GameObject go)
        {
            go.SetActive(true);
        }

        public static void Hide(this GameObject go)
        {
            go.SetActive(false);
        }

        public static T Find<T>(this GameObject go, string name)where T:Object
        {
            return UITool.FindChild<T>(go, name);
        }

    }
//}