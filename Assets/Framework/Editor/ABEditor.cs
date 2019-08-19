using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Framework;
using UnityEditor;
namespace Assets.Framework
{//分为非实例化资源res 和实例化资源prefab
    public class ABEditor:UnityEditor.Editor
    {
        private const string ABConfigPath= "Assets/Editor/ABConfig.asset";
        private static string ABBuildPath = Application.streamingAssetsPath;
        private static List<string> ResPathList = new List<string>();
        private static List<string> prefabPathList = new List<string>();
        [MenuItem("Tools/BuildConfig")]
        static void Build()
        {
            //1.重置静态数据
            ResetData();
            //2.读取配置
            GetConfig();
            GetData();
            //3.打表
            GenerateInfo();
            
        }

        [MenuItem("Tools/BuildAB")]
        static void BuildAB()
        {
            //1.重置静态数据
            ResetData();
            //2.读取配置
            GetConfig();
            GetData();
            //4.设置AB名
            SetABName();
            //5.打包
            ABGenerate();
            //6.清除AB名
            ClearABName();
        }

        static ABConfig abConfig;

        static void ResetData()
        {
            abConfig = null;
            ResPathList.Clear();
            prefabPathList.Clear();
        }
        static void GetConfig()
        {
            abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABConfigPath);
        }
        static void GetData()
        {
            //所有的包名
            //先添加文件夹的包名
            foreach (var ab in abConfig.m_AllFileDirAB)
            { 
                if (ResPathList.Contains(ab.Path))
                {
                    throw new Exception($"重复包名{ab.ABName}");
                }
                //ABNameList.Add(ab.ABName);
                ResPathList.Add(ab.Path);
            }
            //添加Prefab的包名
            string[] allPrefabPathGUID = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
            foreach (var item in allPrefabPathGUID)
            {
                string guidPath = AssetDatabase.GUIDToAssetPath(item);
                //string p = Path.GetFileNameWithoutExtension(guidPath);
                if (ResPathList.Contains(guidPath))
                {
                    throw new Exception($"重复{guidPath}");
                }

                //ABNameList.Add(p);
                prefabPathList.Add(guidPath);
                ResPathList.Add(guidPath);
            }
        }

        static void GenerateInfo()
        {
            //设置表的数据
            MyABConfig config= ScriptableObject.CreateInstance<MyABConfig>();
            List<ABInfo> infoList = new List<ABInfo>();
            //先设置文件夹的ABInfo，这部分没有依赖。
            foreach (var item in abConfig.m_AllFileDirAB)
            {
                ABInfo info = new ABInfo();
                info.path = item.Path;
                info.abName = item.ABName;
                info.resList = new List<ResInfo>();
                var directory = new DirectoryInfo(info.path);
                var files = directory.GetFiles("*", SearchOption.AllDirectories);
                foreach (var f in files)
                {
                    if (f.Name.EndsWith(".meta")) continue;
                    ResInfo res = new ResInfo();
                    res.abName = item.ABName;
                    res.path = item.Path + "/" + f.Name;
                    res.name = Path.GetFileNameWithoutExtension(f.Name);
                    info.resList.Add(res);
                }

                infoList.Add(info);
            }

            //设置prefab的ABInfo 不管资源，但需要处理依赖
            foreach (var item in prefabPathList)
            {
                ABInfo info = new ABInfo();
                info.abName = Path.GetFileNameWithoutExtension(item);
                info.path = item;
                info.dependencies = new List<string>();
                info.resList = new List<ResInfo>();
                //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(item);
                string[] allDepend =AssetDatabase.GetDependencies(item);
                
                foreach (var dp in allDepend)
                {
                    if (dp.EndsWith(".cs")) continue;
                    string fd = Path.GetDirectoryName(dp).Replace(@"\", "/"); // 地址的/符号相反

                    if (ResPathList.Contains(fd))
                    {
                        info.dependencies.Add(Path.GetFileNameWithoutExtension(fd).ToLower());
                    }

                }

                ResInfo res = new ResInfo();
                res.abName = info.abName;
                res.name = Path.GetFileNameWithoutExtension(item);
                res.path = item;
                info.resList.Add(res);

                infoList.Add(info);
            }
            //--------------打表
            config.List = infoList;
            UnityEditor.AssetDatabase.CreateAsset(config, "Assets/MyABConfig.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();


        }

        static void SetABName()
        {
            //先对非实例化资源的文件夹设置
            foreach (var res in ResPathList)
            {
                SetName(res, Path.GetFileNameWithoutExtension(res));
            }
            //prefab需要对自身和所有依赖资源设置包名
            foreach (var pf in prefabPathList)
            {
                //查所有的依赖，进行处理
                string[] allDp=AssetDatabase.GetDependencies(pf);
                foreach (var dp in allDp)
                {
                    if (dp.EndsWith(".cs")) continue;
                    SetName(dp, Path.GetFileNameWithoutExtension(pf));
                }
            }
        }
        static void ClearABName()
        {
            string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
            for (int i = 0; i < oldABNames.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            }

            AssetDatabase.Refresh();
        }
        static void ABGenerate()
        {
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(
                ABBuildPath, 
                BuildAssetBundleOptions.ChunkBasedCompression, 
                EditorUserBuildSettings.activeBuildTarget);
        }
        #region 设置单个文件的包名
        static void SetName(string path,string ABname)
        {
            AssetImporter assetImporter = AssetImporter.GetAtPath(path);
            if (assetImporter == null)
            {
                throw new Exception($"路径不存在{path}");
            }
            else
            {
                assetImporter.assetBundleName = ABname;
            }
        }
        #endregion
    }
}
