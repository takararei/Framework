using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStart : MonoBehaviour {
    public AudioSource source;
    public AudioClip clip;
	// Use this for initialization
	void Start ()
	{
	    ResourceMgr.Instance.Init(this);
	    ResourceMgr.Instance.AsyncLoadResource("Assets/GameData/Sounds/menusound.mp3",OnLoadFinish,LoadResPriority.RES_HIGHT);
	}

    private void OnLoadFinish(string path, Object obj, object param1, object param2, object param3)
    {
        Debug.Log("finish:" + path);
        clip = obj as AudioClip;
        source.clip = clip;
        source.Play();
    }

    // Update is called once per frame
	void Update () {
	    if (Input.GetKeyDown(KeyCode.A))
	    {
	        Debug.Log("release");
	        //source.clip = null;
	        //clip = null;
	        ResourceMgr.Instance.ReleaseResource(clip,true);
	    }
	}

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        Resources.UnloadUnusedAssets();
#endif
    }
}
