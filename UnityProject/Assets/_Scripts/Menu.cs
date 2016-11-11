using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Menu : MonoBehaviour {

    public void LoadLevel(string lvl)
    {
        SceneManager.LoadScene(lvl);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
