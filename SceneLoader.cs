using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void quit()
    {
        Debug.Log("Quitting");

#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif

    }
}
