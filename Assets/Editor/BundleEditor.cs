using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using Assets.Framework.Util;
using UnityEngine;
using UnityEditor;

public class BundleEditor : MonoBehaviour
{
    //根据文件夹打包
    //private static string ABBYTEPATH = RealConfig.GetRealFram().m_ABBytePath;
    private static string m_BundleTargetPath = Application.streamingAssetsPath;
    private static string ABCONFIGPATH = "Assets/Editor/ABConfig.asset";
    private static ABConfig aBConfig;

    //需要设置的ABName的文件夹 ab名,path
    private static Dictionary<string, string> m_NeedSetABNameFolderDict = new Dictionary<string, string>();
    //需要设置ABName的Prefab及其没有ABName的资源
    private static Dictionary<string, List<string>> m_NeedSetABNamePrefabAndResDict = new Dictionary<string, List<string>>();
    //需要设置ABName的所有文件 包括文件夹 地址
    private static List<string> m_AllFileAB = new List<string>();
    //储存所有有效路径 大包的地址  shader sound 所有prefab地址
    private static List<string> m_ConfigFil = new List<string>();//一共就三个 prefab地址，shader sound //记录要打的包源地址


    private static void OnInit()
    {
        m_AllFileAB.Clear();
        m_NeedSetABNameFolderDict.Clear();
        m_NeedSetABNamePrefabAndResDict.Clear();
        m_ConfigFil.Clear();
        aBConfig= AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);
    }
    private static void ReadConfig()
    {

    }
    [MenuItem("Tools/打包")]
    public static void Build()
    {
        OnInit();
        //将所有的config数据都加到dict 以及所有文件夹地址    
        foreach (ABConfig.FileDirABName dir in aBConfig.m_AllFileDirAB)
        {
            if (m_NeedSetABNameFolderDict.ContainsKey(dir.ABName))
            {
                Debug.LogError("AB名字重复："+dir.ABName);
            }
            else
            {
                //记录文件夹的包名和地址
                m_NeedSetABNameFolderDict.Add(dir.ABName, dir.Path);//先记录 shader sound
                
                m_AllFileAB.Add(dir.Path);//记录shader sound文件夹地址
                m_ConfigFil.Add(dir.Path);
            }
        }
        //查找Prefab文件夹下所有的Prefab，获取的是地址的GUID
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", aBConfig.m_AllPrefabPath.ToArray());
        //遍历所有prefab地址GUID
        for (int i = 0; i < allStr.Length; i++)
        {
            //GUID转为地址
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            //进度条
            EditorUtility.DisplayProgressBar("查找prefab", "Prefab:" + path, i * 1.0f / allStr.Length);//进度条
            //每个prefab也打单独的包名
            m_ConfigFil.Add(path);//记下所有prefab 地址
            if (!ContainAllFileAB(path))//一般来说肯定没包含任何prefab地址
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);//加载这个prefab
                string[] allDepend = AssetDatabase.GetDependencies(path);//获取所有依赖的资源的地址
                List<string> allDependPaths = new List<string>();
                for (int j = 0; j < allDepend.Length; j++)
                {
                    //只会记录module下的资源 shader和sound不会记录
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs"))//排除脚本
                    {
                        m_AllFileAB.Add(allDepend[j]);//所有的prefab 目前是 attack shader sound 以及modle里跟attack相关的
                        allDependPaths.Add(allDepend[j]);
                    }
                }

                if (m_NeedSetABNamePrefabAndResDict.ContainsKey(prefab.name))
                {
                    Debug.LogError("存在相同名字的Prefab！名字：" + prefab.name);
                }
                else
                {
                    m_NeedSetABNamePrefabAndResDict.Add(prefab.name, allDependPaths);//记下每个prefab和对应的依赖（除shader.sound
                }
            }

        }

        SetAllABName();//给他们打上ab名， attack shader sound  所有的prefab资源打上对应的ab 如attack
        BuildAssetBundle();
        ClearABName();//清理掉AB名
        ////刷新
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
            //if (!ValidPath(path))//是不是shader sound prefab下的资源 不是的话就不管
            //    continue;
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = Crc32.GetCrc32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/")+1);//资源的名称
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

        string xmlPath = Application.dataPath + "/AssetBundleConfig.xml";
        //SerializeUtil.XMLSerialize(xmlPath, config);
        if (File.Exists(xmlPath)) File.Delete(xmlPath);
        FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
        XmlSerializer xs = new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fileStream.Close();
        ////写入二进制
        foreach (ABBase abBase in config.ABList)
        {
            abBase.Path = "";
        }
        FileStream fs = new FileStream("Assets/GameData/Data/ABData/AssetBundleConfig.bytes", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        fs.Seek(0, SeekOrigin.Begin);
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, config);
        fs.Close();
        //AssetDatabase.Refresh();
        //SetABName("assetbundleconfig", ABBYTEPATH);
    }
    static void BuildAssetBundle()
    {
        string[] allBundlesNames = AssetDatabase.GetAllAssetBundleNames();
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();//路径 包名
        for (int i = 0; i < allBundlesNames.Length; i++)//也就3种包 shader sound attack
        {
            string[] allBundlePaths = AssetDatabase.GetAssetPathsFromAssetBundle(allBundlesNames[i]);
            for (int j = 0; j < allBundlePaths.Length; j++)
            {
                //包括shader和sound下 以及prefab关联的资源
                if (allBundlePaths[j].EndsWith(".cs"))//去掉代码文件
                    continue;

                if(ValidPath(allBundlePaths[j]))
                {
                    resPathDic.Add(allBundlePaths[j], allBundlesNames[i]);
                }
            }
        }

        //删除多余的AB包
        DeleteAB();

        WriteData(resPathDic);
        ////生成自己的配置表

        ////打包
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BundleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }

    static void DeleteAB()
    {
        string[] allBundlesNames = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo direction = new DirectoryInfo(m_BundleTargetPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);//获得streamingasset下所有的文件
        for (int i = 0; i < files.Length; i++)
        {
            if (ContainABName(files[i].Name, allBundlesNames) || files[i].Name.EndsWith(".meta"))//如果文件的名字是那三个或者是meta
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
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && (path.Replace(m_AllFileAB[i], "")[0] == '/')))
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
    }//单文件

    static void SetABName(string name,List<string> paths)//多文件
    {
        for (int i = 0; i < paths.Count; i++)
        {
            SetABName(name, paths[i]);
        }
    }
    //给每个资源文件夹设置包名
    static void SetAllABName()
    {
        foreach (string name in m_NeedSetABNameFolderDict.Keys)
        {
            SetABName(name, m_NeedSetABNameFolderDict[name]);
        }
        //依赖的资源 设置成prefab名的包
        foreach (string name in m_NeedSetABNamePrefabAndResDict.Keys)
        {
            SetABName(name, m_NeedSetABNamePrefabAndResDict[name]);
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
