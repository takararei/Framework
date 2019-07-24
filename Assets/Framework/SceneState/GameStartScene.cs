
using Assets.Framework.UI;
using UnityEngine;
namespace Assets.Framework.SceneState
{
    public class GameStartScene:BaseSceneState
    {
        public override void EnterScene()
        {
            base.EnterScene();
            UIMgr.Instance.Show(UIPanelName.GameStartPanel);
        }
    }
}
