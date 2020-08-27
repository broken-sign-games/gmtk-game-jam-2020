using UnityEngine;

public class Exit : MonoBehaviour
{
    public void StopOrQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
