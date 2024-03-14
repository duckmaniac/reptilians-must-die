using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private bool isReptilian;
    private TextMeshProUGUI textMeshPro;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnOpponentHealthChanged += ChangeHealthText;
        EventBus.Instance.OnOpponentReceivedDamage += StartHiglightAnimation;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnOpponentHealthChanged -= ChangeHealthText;
        EventBus.Instance.OnOpponentReceivedDamage -= StartHiglightAnimation;
    }

    private void ChangeHealthText(int value, bool isReptilian)
    {
        if (this.isReptilian == isReptilian) textMeshPro.text = value.ToString();
    }

    private void StartHiglightAnimation(bool isReptilian)
    {
        if (this.isReptilian == isReptilian) StartCoroutine(HiglightAnimation());
    }

    private IEnumerator HiglightAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            textMeshPro.faceColor = Constants.COLOR_RED;
            yield return new WaitForSeconds(Constants.TIME_HIGHLIGHT_ANIMATION / 2);
            textMeshPro.faceColor = Color.white;
            yield return new WaitForSeconds(Constants.TIME_HIGHLIGHT_ANIMATION / 2);
        }
    }
}
