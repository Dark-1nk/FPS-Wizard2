using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetStats();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Cutscene");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
