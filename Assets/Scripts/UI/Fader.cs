using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    private Image image;
    private bool isWorking = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        EventBus.Instance.OnFadeIn += StartFadeIn;
        EventBus.Instance.OnFadeOut += StartFadeOut;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnFadeIn -= StartFadeIn;
        EventBus.Instance.OnFadeOut -= StartFadeOut;
    }

    private IEnumerator FadeAnimation(float startAlpha, float endAlpha, float duration)
    {
        isWorking = true;
        float time = 0f;
        Color startColor = image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, endAlpha);

        while (time < duration)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            image.color = Color.Lerp(new Color(startColor.r, startColor.g, startColor.b, startAlpha), endColor, normalizedTime);
            yield return null;
        }
        image.color = endColor;
        isWorking = false;
    }

    private void StartFadeIn()
    {
        if (isWorking) return;
        StartCoroutine(FadeAnimation(0f, 1f, Constants.TIME_FADE_ANIMATION));
    }

    private void StartFadeOut()
    {
        if (isWorking) return;
        StartCoroutine(FadeAnimation(1f, 0f, Constants.TIME_FADE_ANIMATION));
    }
}
