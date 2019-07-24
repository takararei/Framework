using UnityEngine;
namespace Assets.Framework.Util
{
    public partial class CommonUtil
    {
        public static void CopyText(string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }
    }
}

