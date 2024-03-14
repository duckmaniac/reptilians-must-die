using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI textMeshPro;
    private Coroutine currentCountCoroutine;

    private void OnEnable()
    {
        EventBus.Instance.OnTurnStarted += StartCount;
        EventBus.Instance.OnBattleStarted += StopCount;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnTurnStarted -= StartCount;
        EventBus.Instance.OnBattleStarted -= StopCount;
    }

    private void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void StartCount()
    {
        StopCount();
        currentCountCoroutine = StartCoroutine(Count()); 
    }

    private void StopCount()
    {
        if (currentCountCoroutine != null)
        {
            StopCoroutine(currentCountCoroutine);
        }
    }

    private IEnumerator Count()
    {
        int time = Constants.TIME_TURN_DURATION;
        while (time > 0)
        {
            textMeshPro.text = time.ToString();
            yield return new WaitForSeconds(1f);
            time--;
        }
        EventBus.Instance.RaiseOnTurnEnded();
    }
}
