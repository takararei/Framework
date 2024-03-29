﻿using System;
using System.Collections.Generic;

namespace Assets.Framework.Extension
{
    /// <summary>
    /// 对Dictionary扩展
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 尝试根据Key得到Value,没有就返回null
        /// </summary>
        public static Tvalue TryGet<Tkey,Tvalue>(this Dictionary<Tkey,Tvalue>dict, Tkey key)
        {
            Tvalue value;
            dict.TryGetValue(key, out value);
            return value;
        } 

        public static bool Contain<Tkey,Tvalue>(this Dictionary<Tkey,Tvalue>dict,Tkey key)
        {
            Tvalue value;
            return dict.TryGetValue(key, out value);
        }
    }

    public static class StringExtension
    {
        public static byte[] ToBytes(this string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }
    }
}
