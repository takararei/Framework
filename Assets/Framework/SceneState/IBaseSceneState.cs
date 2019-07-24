using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Framework.SceneState
{
    public interface IBaseSceneState
    {
        SceneName scene { get; set; }
        void EnterScene();
        void ExitScene();
    }
}