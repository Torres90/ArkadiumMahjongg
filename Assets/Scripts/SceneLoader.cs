using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //[SerializeField] private string startupScene;

    private void OnEnable()
    {
        StartGameButton.LoadMainScene += OnLoadMainScene;
    }
    private void OnDisable()
    {
        StartGameButton.LoadMainScene += OnLoadMainScene;        
    }

    private void OnLoadMainScene(object sender, EventArgs e)
    {
        SceneManager.UnloadSceneAsync("IntroScene");
        SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
        Debug.Log($"Active scene is {SceneManager.GetActiveScene().name}");

    }

    void Start()
    {
        //if (Application.isEditor)
        //{
        //    Scene loadedLevel = SceneManager.GetSceneByName("Level 1");
        //    if (loadedLevel.isLoaded)
        //    {
        //        SceneManager.SetActiveScene(loadedLevel);
        //        return;
        //    }
        //}

        //Only load intro scene at start if the Base Scene is the active scene.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneAt(0))
        {
            SceneManager.LoadScene("IntroScene", LoadSceneMode.Additive);
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("IntroScene"));
            Debug.Log($"Active scene is {SceneManager.GetActiveScene().name}");
        }

    }

}
