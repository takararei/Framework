
using Assets.Framework.Singleton;
using UnityEngine;

namespace Assets.Framework.Res
{

    public class ResMgr:Singleton<ResMgr>
    {
        private IBaseRes resFactory;
        public override void Init()
        {
            resFactory = new BaseRes();
        }

        public T GetRes<T>(string resPath) where T : Object
        {
            return resFactory.GetRes<T>(resPath);
        }

        

    }
}
