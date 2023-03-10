using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour
{
    public void Exit()
    {
        Debug.Log("out");
        Application.Quit();
    }
    public void toGame()
    {
        SceneManager.LoadScene("test");
    }
    public void toMenu()
    {
        SceneManager.LoadScene("menu");
    }
}
