using Assets.Framework.Audio;
using Assets.Framework.Res;
using Assets.Framework.SceneState;
using Assets.Framework.UI;
using UnityEngine;

namespace Assets.Framework
{
    public class GameRoot:MonoBehaviour
    {
        private static GameRoot _instance;
        public static GameRoot Instance
        {
            get
            {
                return _instance;
            }
        }
        private void Awake()
        {
            DontDestroyOnLoad(this);
            _instance = this;
            ResMgr.Instance.Init();
            AudioMgr.Instance.Init();
            UIMgr.Instance.Init();
            SceneStateMgr.Instance.Init();
            SceneStateMgr.Instance.GoToScene(SceneName.GameStart);

        }

        private void Start()
        {
            
        }

        private void Update()
        {
            UIMgr.Instance.Update();
            
        }

        public GameObject CreateItem(GameObject itemGo)
        {
            GameObject go = Instantiate(itemGo);
            return go;
        }



    }
}
