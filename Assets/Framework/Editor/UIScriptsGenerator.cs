using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class UIScriptsGenerator : Editor
{

    public enum UIItemType
    {
        Txt,
        Img,
        Btn,
        Raw,
        Trans,
        //添加新项时需要修改GetTypeName GetUIItem方法

    }

    static string scriptName;
    static string filePath = "Assets/Scripts/UIPanel/";
    static List<string> uiItemNames = new List<string>();
    static List<UIItemType> uiItemTypes = new List<UIItemType>();



    [MenuItem("GameObject/UITools/CreateScripts", priority = 0)]
    static void CreateScripts()
    {
        //选中 并且选择一个
        if (Selection.gameObjects != null && Selection.gameObjects.Length == 1)
        {
            uiItemNames.Clear();
            uiItemTypes.Clear();
            GameObject go = Selection.gameObjects[0];
            scriptName = go.name;
            //获取这个物体下的所有子物体。
            GetUIItem(go);

            CreateScript();
        }



    }

    static void GetUIItem(GameObject go)
    {
        Transform[] transArr = go.GetComponentsInChildren<Transform>();
        foreach (var item in transArr)
        {
            //查找出名称前缀是Txt,Btn,Img的物体 添加到list 即命名为Txt_test 这类的
            string[] strArr = item.name.Split('_');
            if (strArr[0] == "Txt")
            {
                uiItemNames.Add(item.name);
                uiItemTypes.Add(UIItemType.Txt);
            }
            else if (strArr[0] == "Btn")
            {
                uiItemNames.Add(item.name);
                uiItemTypes.Add(UIItemType.Btn);
            }
            else if (strArr[0] == "Img")
            {
                uiItemNames.Add(item.name);
                uiItemTypes.Add(UIItemType.Img);
            }
        }
    }
    static void CreateScript()
    {
        if (uiItemNames.Count == 0) return;
        StringBuilder sb = new StringBuilder();
        //命名空间
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using System;");
        sb.AppendLine("using Assets.Framework.UI;");
        sb.AppendLine();
        //开始
        sb.Append("public class ");
        sb.Append(scriptName);//类名
        sb.Append(" : BasePanel");//继承
        sb.AppendLine();
        //类的具体内容
        sb.AppendLine("{");

        //写入变量--------------------------------------------------------
        // private 类型 m物体名;
        for (int i = 0; i < uiItemNames.Count; i++)
        {
            StringBuilder sb_variable = new StringBuilder();
            //获取类型名
            string typeName = GetTypeName(i);
            sb_variable.Append("    private ");
            sb_variable.Append(typeName);
            sb_variable.Append(" m");//加个m表示私有变量
            sb_variable.Append(uiItemNames[i]);
            sb_variable.Append(";");
            sb.AppendLine(sb_variable.ToString());
        }
        //----------------------------------------------------------------

        //写入方法

        sb.AppendLine();
        sb.AppendLine("    public override void Init()");
        sb.AppendLine("    {");
        sb.AppendLine("        base.Init();");
        //开始对变量赋值-------------------------------------------------
        for (int i = 0; i < uiItemNames.Count; i++)
        {
            StringBuilder sb_variable = new StringBuilder();
            string typeName = GetTypeName(i);
            sb_variable.Append("        m");
            sb_variable.Append(uiItemNames[i]);
            sb_variable.Append(" = Find<");
            sb_variable.Append(typeName);
            sb_variable.Append(">(\"");//>("
            sb_variable.Append(uiItemNames[i]);
            sb_variable.Append("\");");//");
            sb.AppendLine(sb_variable.ToString());
        }
        //----------------------------------------------------------------
        sb.AppendLine("    }");

        sb.AppendLine();
        sb.AppendLine("    public override void OnShow()");
        sb.AppendLine("    {");
        sb.AppendLine("        base.OnShow();");
        sb.AppendLine("    }");

        sb.AppendLine();
        sb.AppendLine("    public override void OnHide()");
        sb.AppendLine("    {");
        sb.AppendLine("        base.OnHide();");
        sb.AppendLine("    }");

        sb.AppendLine();
        sb.AppendLine("    public override void OnDestroy()");
        sb.AppendLine("    {");
        sb.AppendLine("        base.OnDestroy();");
        sb.AppendLine("    }");




        sb.AppendLine("}");
        WriteStrToFile(sb.ToString());
        AssetDatabase.Refresh();
    }
    static string GetTypeName(int i)
    {
        switch (uiItemTypes[i])
        {
            case UIItemType.Txt:
                return "Text";
            case UIItemType.Img:
                return "Image";
            case UIItemType.Btn:
                return "Button";
            case UIItemType.Raw:
                return "RawImage";
            case UIItemType.Trans:
                return "Transform";
        }

        return null;
    }
    static void WriteStrToFile(string txt)
    {
        if (string.IsNullOrEmpty(txt) || string.IsNullOrEmpty(filePath))
        {
            Debug.Log("文本或者文件地址有误");
            return;
        }
        File.WriteAllText(filePath + scriptName + ".cs", txt, Encoding.UTF8);
    }
}
