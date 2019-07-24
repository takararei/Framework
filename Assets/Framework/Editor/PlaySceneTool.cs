
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Assets.Framework.Editor
{
    public class PlaySceneTool
    {
        [MenuItem("Tools/PlayModeUseFirstScene", true)]
        static bool ValidatePlayModeUseFirstScene()
        {
            Menu.SetChecked("Tools/PlayModeUseFirstScene", EditorSceneManager.playModeStartScene != null);
            return !EditorApplication.isPlaying;
        }

        [MenuItem("Tools/PlayModeUseFirstScene")]
        static void UpdatePlayModeUseFirstScene()
        {
            if (Menu.GetChecked("Tools/PlayModeUseFirstScene"))
            {
                EditorSceneManager.playModeStartScene = null;
            }
            else
            {
                SceneAsset scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(EditorBuildSettings.scenes[0].path);
                EditorSceneManager.playModeStartScene = scene;
            }
        }
    }
}
