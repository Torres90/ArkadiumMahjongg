using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject LoadingCanvas;

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

        StartCoroutine(WaitForLoadSceneAsync("MainScene"));

        //SceneManager.LoadScene("MainScene", LoadSceneMode.Additive);
        //Debug.Log($"Active scene is {SceneManager.GetActiveScene().name}");

    }

    void Start()
    {

        //Only load intro scene at start if the Base Scene is the active scene.
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneAt(0))
        {
            StartCoroutine(WaitForLoadSceneAsync("IntroScene"));
            //SceneManager.LoadScene("IntroScene", LoadSceneMode.Additive);
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("IntroScene"));
            Debug.Log($"Active scene is {SceneManager.GetActiveScene().name}");
        }

    }

    IEnumerator WaitForLoadSceneAsync(string sceneName)
    {
        LoadingCanvas.SetActive(true);
        LoadingCanvas.GetComponent<Canvas>().sortingOrder = 100;
        // The Application loads the Scene in the background as the current Scene runs.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        //yield return new WaitForSeconds(3); //Debug only

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        LoadingCanvas.SetActive(false);
    }
}
