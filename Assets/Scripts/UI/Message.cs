using System.Collections;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private bool isWorking = false;

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnShowMessage += Show;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnShowMessage -= Show;
    }

    private IEnumerator ShowAnimation()
    {
        isWorking = true;
        float time = 0f;
        Color startColor = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 0f);
        Color endColor = new Color(textMeshPro.color.r, textMeshPro.color.g, textMeshPro.color.b, 1f);

        // fade text in
        EventBus.Instance.RaiseOnFadeIn();
        while (time < Constants.TIME_FADE_ANIMATION)
        {
            time += Time.deltaTime;
            float normalizedTime = time / Constants.TIME_FADE_ANIMATION;
            textMeshPro.color = Color.Lerp(startColor, endColor, normalizedTime);
            yield return null;
        }
        textMeshPro.color = endColor;

        // wait
        yield return new WaitForSeconds(Constants.TIME_TEXT_DISPLAY_DURATION);

        // fade text out
        EventBus.Instance.RaiseOnFadeOut();
        time = 0f;
        while (time < Constants.TIME_FADE_ANIMATION)
        {
            time += Time.deltaTime;
            float normalizedTime = time / Constants.TIME_FADE_ANIMATION;
            textMeshPro.color = Color.Lerp(endColor, startColor, normalizedTime);
            yield return null;
        }
        textMeshPro.color = startColor;
        isWorking = false;
    }

    private void Show(string message)
    {
        if (isWorking) return;
        textMeshPro.text = message;
        StartCoroutine(ShowAnimation());
    }
}
