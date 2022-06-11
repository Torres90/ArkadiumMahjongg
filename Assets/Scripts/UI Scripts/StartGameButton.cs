using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameButton : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene(1, LoadSceneMode.Additive); //Scene 1 is the Main Scene
}
