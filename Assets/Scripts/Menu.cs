using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.GetComponentInChildren<AudioSource>().Play();
    }
    public void Continue()
    {
        SceneManager.LoadScene("News");
    }

    public void NewGame()
    {
        GameManager.Instance.CurrentLevelNumber = 1;
        SceneManager.LoadScene("News");
    }
}
