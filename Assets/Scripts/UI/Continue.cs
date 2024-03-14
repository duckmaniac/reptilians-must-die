using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Continue : MonoBehaviour
{
    private Button button;
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        if (GameManager.Instance.CurrentLevelNumber == 1)
        {
            button = GetComponent<Button>();
            textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
            button.interactable = false;
            textMeshPro.faceColor = Color.gray;
        }  
    }
}
