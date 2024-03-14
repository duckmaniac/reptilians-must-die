using UnityEngine;
using UnityEngine.SceneManagement;

public class End : MonoBehaviour
{
    public void Okay()
    {
        SceneManager.LoadScene("Menu");
    }
}
