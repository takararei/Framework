using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using UnityEngine;
using UnityEditor;

public class BundleEditor : MonoBehaviour
{
    //private static string ABBYTEPATH = RealConfig.GetRealFram().m_ABBytePath;
    private static string m_BunleTargetPath = Application.streamingAssetsPath;
    private static string ABCONFIGPATH = "Assets/Editor/ABConfig.asset";
    //ab名,path
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
    //ab文件夹地址
    private static List<string> m_AllFileAB = new List<string>();
    //每个prefab 和对应的依赖地址
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();
    private static ABConfig aBConfig= AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
    //储存所有有效路径
    private static List<string> m_ConfigFil = new List<string>();
    [MenuItem("Tools/打包")]
    public static void Build()
    {
        m_AllFileAB.Clear();
        m_AllFileDir.Clear();
        m_AllPrefabDir.Clear();
        m_ConfigFil.Clear();
        //将所有的config数据都加到dict 以及所有文件夹地址    
        foreach (ABConfig.FileDirABName dir in aBConfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(dir.ABName))
            {
                Debug.LogError("AB名字重复："+dir.ABName);
            }
            else
            {
                m_AllFileDir.Add(dir.ABName, dir.Path);
                m_AllFileAB.Add(dir.Path);
                m_ConfigFil.Add(dir.Path);
            }
        }
        //查找所有Prefab文件夹下所有的Prefab，获取的是地址的GUID
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", aBConfig.m_AllPrefabPath.ToArray());
        //遍历所有prefab地址GUID
        for (int i = 0; i < allStr.Length; i++)
        {

            //GUID转为地址
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            //进度条
            EditorUtility.DisplayProgressBar("查找prefab", "Prefab:" + path, i * 1.0f / allStr.Length);//进度条
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);//加载这个prefab
                string[] allDepend = AssetDatabase.GetDependencies(path);//获取所有依赖的资源的地址
                List<string> allDependPath = new List<string>();
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))//排除脚本
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }

                if (m_AllPrefabDir.ContainsKey(prefab.name))
                {
                    Debug.LogError("存在相同名字的Prefab！名字：" + prefab.name);
                }
                else
                {
                    m_AllPrefabDir.Add(prefab.name, allDependPath);
                }
            }

        }
       
        SetAllABName();
        BuildAssetBundle();
        ClearABName();
        //刷新
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        //清除进度条
        EditorUtility.ClearProgressBar();
    }

    static void WriteData(Dictionary<string,string> resPathDic)
    {

        AssetBundleConfig config = new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach(var path in  resPathDic.Keys)
        {
            if (!ValidPath(path))
                continue;
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = Crc32.GetCrc32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/")+1);
            abBase.ABDependence = new List<string>();
            string[] resDependece = AssetDatabase.GetDependencies(path);
            for (int i = 0; i < resDependece.Length; i++)
            {
                string tempPath = resDependece[i];

                if (tempPath == path || path.EndsWith(".cs"))
                    continue;

                string abName = "";
                if (resPathDic.TryGetValue(tempPath, out abName))
                {
                    if (abName == resPathDic[path])
                        continue;

                    if (!abBase.ABDependence.Contains(abName))
                    {
                        abBase.ABDependence.Add(abName);
                    }
                }
            }
            config.ABList.Add(abBase);
        }
        //写入xml
        string xmlPath = Application.dataPath + "/AssetbundleConfig.xml";
        if (File.Exists(xmlPath)) File.Delete(xmlPath);
        FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xs = new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fileStream.Close();
        //写入二进制
        foreach (ABBase abBase in config.ABList)
        {
            abBase.Path = "";
        }
        //FileStream fs = new FileStream(ABBYTEPATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        //fs.Seek(0, SeekOrigin.Begin);
        //fs.SetLength(0);
        //BinaryFormatter bf = new BinaryFormatter();
        //bf.Serialize(fs, config);
        //fs.Close();
        //AssetDatabase.Refresh();
        //SetABName("assetbundleconfig", ABBYTEPATH);
    }
    static void BuildAssetBundle()
    {
        string[] allBundlesNames = AssetDatabase.GetAllAssetBundleNames();
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();//路径 包名
        for (int i = 0; i < allBundlesNames.Length; i++)
        {
            string[] allBundlePaths = AssetDatabase.GetAssetPathsFromAssetBundle(allBundlesNames[i]);
            for (int j = 0; j < allBundlePaths.Length; j++)
            {
                if (allBundlePaths[j].EndsWith(".cs"))
                    continue;
                //Debug.Log("此AB包：" + allBundles[i] + "下面包含的资源文件路径：" + allBundlePath[j]);
                resPathDic.Add(allBundlePaths[j], allBundlesNames[i]);
            }
        }
        //删除多余的AB包
        DeleteAB();

        WriteData(resPathDic);
        //生成自己的配置表

        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BunleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    static void DeleteAB()
    {
        string[] allBundlesNames = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo direction = new DirectoryInfo(m_BunleTargetPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (ContainABName(files[i].Name, allBundlesNames) || files[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            else
            {
                Debug.Log("此AB包已经被删或者改名了：" + files[i].Name);
                if (File.Exists(files[i].FullName))
                {
                    File.Delete(files[i].FullName);
                }
                if (File.Exists(files[i].FullName + ".manifest"))
                {
                    File.Delete(files[i].FullName + ".manifest");
                }
            }
        }
    }

    static bool ContainABName(string name, string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (name==strs[i])
            {
                return true;
            }
        }

        return false;
    }

    //清除掉AB名 以免被版本控制记录meta带来不便
    static void ClearABName()
    {
        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < oldABNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[i], i * 1.0f / oldABNames.Length);
        }
    }

    static bool ContainAllFileAB(string path)
    {
        for (int i = 0; i < m_AllFileAB.Count; i++)
        {
            //如果这个资源地址 已经存在 或者就是某个被记录文件夹下的 就不需要再打包
            if (path == m_AllFileAB[i] || path.Contains(m_AllFileAB[i]))
                return true;
        }
        return false;
    }

    static void SetABName(string name,string path)
    {
        //给资源设置AB名
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null)
        {
            Debug.LogError("不存在此路径的文件：" + path);
        }
        else
        {
            assetImporter.assetBundleName = name;
        }
    }

    static void SetABName(string name,List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            SetABName(name, paths[i]);
        }
    }
    //给每个资源文件夹设置包名
    static void SetAllABName()
    {
        foreach (string name in m_AllFileDir.Keys)
        {
            SetABName(name, m_AllFileDir[name]);
        }
        //依赖的资源 设置成prefab名的包
        foreach (string name in m_AllPrefabDir.Keys)
        {
            SetABName(name, m_AllPrefabDir[name]);
        }
    }

    /// <summary>
    /// 是否有效路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        for (int i = 0; i < m_ConfigFil.Count; i++)
        {
            if (path.Contains(m_ConfigFil[i]))
            {
                return true;
            }
        }
        return false;
    }
}
