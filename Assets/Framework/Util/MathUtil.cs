using System.Collections.Generic;
using UnityEngine;

namespace Assets.Framework.Util
{
    public partial class MathUtil
    {

        /// <summary>
        /// 是否在概率内
        /// </summary>
        /// <param name="percent">概率范围</param>
        /// <returns></returns>
        public static bool Percent(int percent)
        {
            return UnityEngine.Random.Range(0, 100) <= percent;
        }

        /// <summary>
        /// 从数组中随机选择一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">传数组或者多个参数</param>
        /// <returns></returns>
        public static T GetRandomValueFrom<T>(params T[] values)
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
        /// <summary>
        /// 从List中随机选择一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public static T GetRandomValueFrom<T>(List<T> values)
        {
            return values[UnityEngine.Random.Range(0, values.Count)];
        }
    }

}

