using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Assets.Framework.SceneState
{
    public interface IBaseSceneState
    {
        void EnterScene();
        void ExitScene();
    }
}