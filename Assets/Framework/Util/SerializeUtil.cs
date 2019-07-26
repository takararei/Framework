using System;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Assets.Framework.Util
{
    public static class SerializeUtil
    {
        public static bool XMLSerialize(string path, System.Object obj)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8))
                    {
                        XmlSerializer xs = new XmlSerializer(obj.GetType());
                        xs.Serialize(sw, obj);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("此类无法转换成xml " + obj.GetType() + "," + e);
            }
            return false;
        }
        
        public static bool BinarySerilize(string path, System.Object obj)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, obj);
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("此类无法转换成二进制 " + obj.GetType() + "," + e);
            }
            return false;
        }

        public static T XMLDeserialize<T>(string path) where T : class
        {
            T t = default(T);
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    t = (T)xs.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("此xml无法转成对象: " + path + "," + e);
            }
            return t;
        }

        public static T BinaryDeserilize<T>(string path) where T : class
        {
            T t = default(T);
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    t=(T)bf.Deserialize(fs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("此bytes无法转成对象: " + path + "," + e);
            }
            return t;
        }
    }
}
