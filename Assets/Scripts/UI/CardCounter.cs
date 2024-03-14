using System.Collections;
using TMPro;
using UnityEngine;

public class CardCounter : MonoBehaviour
{
    [SerializeField] private bool isReptilian;
    private TextMeshProUGUI textMeshPro;

    private void OnEnable()
    {
        EventBus.Instance.OnHandOverflow += StartHighlightAnimation;
        EventBus.Instance.OnNumberOfCardsInHandChanged += SetValue;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnHandOverflow -= StartHighlightAnimation;
        EventBus.Instance.OnNumberOfCardsInHandChanged -= SetValue;
    }

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void StartHighlightAnimation(bool isReptilian)
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

    private void SetValue(int counter, bool isReptilian)
    {
        if (this.isReptilian == isReptilian) textMeshPro.text = $"{counter}/{Constants.MAX_CARDS_IN_HAND}";
    }
}
