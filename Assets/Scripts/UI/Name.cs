using TMPro;
using UnityEngine;

public class Name : MonoBehaviour
{
    [SerializeField] private bool isReptilian;
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnNameChanged += ChangeNameText;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnNameChanged -= ChangeNameText;
    }

    private void ChangeNameText(string value, bool isReptilian)
    {
        if (this.isReptilian == isReptilian) textMeshPro.text = value.ToString();
    }
}
