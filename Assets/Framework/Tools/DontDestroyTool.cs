using UnityEngine;

public class DontDestroyTool : MonoBehaviour 
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
}