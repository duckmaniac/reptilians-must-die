using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class News : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshPro;
    private string news;

    private void Awake()
    {
        news = Utility.LoadJsonFromResources<string>("News/" + GameManager.Instance.CurrentLevelNumber.ToString());
        textMeshPro.text = news;
    }

    public void Okay()
    {
        GameManager.Instance.NextLevel();
    }
}
