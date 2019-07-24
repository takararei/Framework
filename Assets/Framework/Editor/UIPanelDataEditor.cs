using Assets.Framework.Excel;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
namespace Assets.Framework.UI
{
    public class UIPanelDataEditor: BaseExcelEditor
    {
        [UnityEditor.MenuItem("ExcelToAsset/UIPanelData")]
        public static void ExcelToAsset()
        {
            fileName = "UIPanelData";
            UIPanelDataMgr mgr = ScriptableObject.CreateInstance<UIPanelDataMgr>();
            mgr.PanelInfoList = ReadExcel(excelPath);

            CreateAsset(mgr);
        }

        public static List<UIPanelInfo> ReadExcel(string ePath)
        {
            List<UIPanelInfo> testDict = new List<UIPanelInfo>();
            using (FileStream stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    excelReader.Read();
                    //除了第一行的字段名 和最后一行空白
                    for (int i = 1; i < excelReader.RowCount; i++)
                    {
                        excelReader.Read();
                        UIPanelInfo pdata = new UIPanelInfo();
                        //数字类型需要转换
                        pdata.id = int.Parse(excelReader.GetString(0));
                        pdata.panelName = excelReader.GetString(1);
                        pdata.path = excelReader.GetString(2);
                        pdata.layer = excelReader.GetString(3);

                        testDict.Add(pdata);

                    }
                }
            }

            return testDict;
        }
    }
}
#endif