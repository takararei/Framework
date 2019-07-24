using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
namespace Assets.Framework.Excel
{
    public class BaseExcelEditor : UnityEditor.Editor
    {
        static IExcelDataReader reader;
        public static string excelPath
        {
            get
            {
                return "../MyGraduation/Excel/" + fileName + ".xlsx";
            }
        }

        public static string savePath= "Assets/Resources/AssetData/";

        public static string fileName;

        public static void CreateAsset(UnityEngine.Object mgr)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            UnityEditor.AssetDatabase.CreateAsset(mgr, savePath + fileName + ".asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            Debug.Log(fileName + "生成成功");
        }
        public static void SetReader(IExcelDataReader ireader)
        {
            reader = ireader;
        }
        
        public static string GetString(int index)
        {
            return reader.GetString(index);
        }

        public static int GetInt(int index)
        {
            return int.Parse(GetString(index));
        }

        public static float GetFloat(int index)
        {
            return float.Parse(GetString(index));
        }
    }
}
#endif