using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Framework.Extension
{
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

    }
}