using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Okay : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;

    private void Start()
    {
        textMeshPro = GetComponentInChildren<TextMeshProUGUI>();
        StartCoroutine(ShowAnimation());
    }

    private IEnumerator ShowAnimation()
    {
        yield return new WaitForSeconds(3f);
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
    }
}
