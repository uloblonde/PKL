using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageMenu : MonoBehaviour
{
    public void Stage01()
    {
        SceneManager.LoadScene(2);
    }

    public void Stage02()
    {
        SceneManager.LoadScene(5);
    }

    public void Stage03()
    {
        SceneManager.LoadScene(8);
    }

    public void BackMenu()
    {
        SceneManager.LoadScene(0);
    }
}