using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void BTN_PlayGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void BTN_ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
