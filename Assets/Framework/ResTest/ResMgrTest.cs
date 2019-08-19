using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Framework.Singleton;

namespace Assets.Framework.ResTest
{
    public enum LoadResPriority
    {
        RES_HIGHT = 0,//最高优先级
        RES_MIDDLE,//一般优先级
        RES_SLOW,//低优先级
        RES_NUM,
    }

    public class ResMgrTest:Singleton<ResMgrTest>
    {
        private Dictionary<uint, ResItem> resItemDict = new Dictionary<uint, ResItem>();
        private Dictionary<uint, ABItem> abItemDict = new Dictionary<uint, ABItem>();
        private ClassObjectPool<ABItem> abItemPool = new ClassObjectPool<ABItem>(100);

        //最长连续卡着加载资源的时间，单位微秒
        private const long MAXLOADRESTIME = 200000;

        //最大缓存个数
        private const int MAXCACHECOUNT = 500;

        public override void Init()
        {
            base.Init();
        }

        public void LoadConfig()
        {
            string path = Application.streamingAssetsPath+"/myabconfig";
            AssetBundle configAB = AssetBundle.LoadFromFile(path);
            MyABConfig config = configAB.LoadAsset<MyABConfig>("myabconfig");
            if (config == null)
            {
                throw new Exception("config is not exist");
            }

            foreach (var item in config.List)
            {
                foreach (var res in item.resList)
                {
                    var resItem = new ResItem
                    {
                        AssetName = res.name,
                        Crc = Crc32.GetCrc32(res.path),
                        ABName = item.abName,
                        DependentAB = item.dependencies
                    };
                    ResItem tempRes;
                    if (!resItemDict.TryGetValue(resItem.Crc, out tempRes))
                    {
                        resItemDict.Add(resItem.Crc, resItem);

                    }
                    else
                    {
                        throw new Exception($"资源重复{resItem.AssetName}");
                    }
                }
            }
        }

        public AssetBundle LoadAB(string name)
        {
            ABItem item = null;
            uint crc = Crc32.GetCrc32(name);
            if (!abItemDict.TryGetValue(crc, out item))
            {
                AssetBundle ab = null;
                string fullPath = Application.streamingAssetsPath + $"/{name}";
                ab = AssetBundle.LoadFromFile(fullPath);
                if (ab == null)
                {
                    throw new Exception($"ab 加载失败:{fullPath}");
                }

                item = abItemPool.Spawn();
                item.AB = ab;
                item.Retain();
                abItemDict.Add(crc, item);
            }
            else
            {
                item.Retain();
            }

            return item.AB;
        }

        public void UnloadAB(string name)
        {
            ABItem item = null;
            var crc = Crc32.GetCrc32(name);
            if (abItemDict.TryGetValue(crc, out item)&&item !=null)
            {
                item.Release();
                if (item.RefCount > 0) return;
                abItemPool.Recycle(item);
                abItemDict.Remove(crc);
            }
            else
            {
                Debug.LogError($"无法释放AB:{name}");
            }
        }

        public ResItem LoadResAB(uint crc)
        {
            ResItem item = GetResItem(crc);
            if (item == null)
            {
                Debug.LogError($"res is null :{crc}");
                return item;
            }

            if (item.AB != null) return item;

            item.AB = LoadAB(item.ABName);

            if (item.DependentAB == null) return item;

            foreach (var ab in item.DependentAB)
            {
                LoadAB(ab);
            }

            return item;
        }

        public void UnloadRes(ResItem item)
        {
            if (item?.DependentAB == null || item.DependentAB.Count <= 0) return;
            foreach (var dpAB in item.DependentAB)
            {
                UnloadAB(dpAB);
            }

            UnloadAB(item.ABName);

        }

        public ResItem GetResItem(uint crc)
        {
            ResItem item = null;
            resItemDict.TryGetValue(crc, out item);
            return item;
        }
    }
}
