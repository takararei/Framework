using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BundleEditor : MonoBehaviour
{
    public static string ABCONFIGPATH = "Assets/Editor/ABConfig.asset";
    public static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
    [MenuItem("Tools/打包")]
    public static void Build()
    {
        ABConfig aBConfig=AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
        foreach (string str in aBConfig.m_AllPrefabPath)
        {
        }
        foreach (ABConfig.FileDirABName dir in aBConfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(dir.ABName))
            {
                Debug.LogError("AB名字重复："+dir.ABName);
            }
            else
            {
                m_AllFileDir.Add(dir.ABName, dir.Path);
            }
        }
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", aBConfig.m_AllPrefabPath.ToArray());

    }
}
