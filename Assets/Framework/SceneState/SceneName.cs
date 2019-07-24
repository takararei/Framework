using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Framework.SceneState
{
    public static class SceneType
    {
        public static IBaseSceneState GetScene(SceneName scene)
        {
            IBaseSceneState state = null;
            SceneDict.TryGetValue(scene, out state);
            return state;
        }

        private static Dictionary<SceneName, IBaseSceneState> SceneDict = new Dictionary<SceneName, IBaseSceneState>
        {
            {SceneName.GameStart,new GameStartScene() },
        };
        
    }

    public enum SceneName
    {
        GameStart = 0,
    }
}
