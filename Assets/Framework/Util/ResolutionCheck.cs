using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Framework.Util
{
    public partial class ResolutionCheck
    {
        public static float GetAspectRatio()
        {
            return
                Screen.width > Screen.height ?
                (float)Screen.width / Screen.height :
                (float)Screen.height / Screen.width;
        }

        public static bool IsPadResolution()
        {
            var aspect = GetAspectRatio();
            return aspect > (4.0f / 3 - 0.05) && aspect < (4.0f / 3 + 0.05);
        }

        public static bool IsPhoneResolution()
        {
            var aspect = GetAspectRatio();
            return aspect > 16.0f / 9 - 0.05 && aspect < 16.0f / 9 + 0.05;
        }
        /// <summary>
        /// 是否是iPhone X 分辨率 2436:1125
        /// </summary>
        /// <returns></returns>
        public static bool IsiPhoneXResolution()
        {
            var aspect = GetAspectRatio();
            return aspect > 2436.0f / 1125 - 0.05 && aspect < 2436.0f / 1125 + 0.05;
        }
    }

}
