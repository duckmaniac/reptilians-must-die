using System.Collections;
using UnityEngine;
using UnityEngine.UI;

enum ChangedEvent
{
    OnSpawnPointsChanged,
    OnSpawnPointsUnusedChanged
}

public class SpawnPoints : MonoBehaviour
{
    [SerializeField] private bool isReptilian;
    [SerializeField] private ChangedEvent selectedEvent;
    private static Sprite[] sprites;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        sprites ??= Resources.LoadAll<Sprite>("SpawnPoints");
    }

    private void OnEnable()
    {
        switch (selectedEvent)
        {
            case ChangedEvent.OnSpawnPointsChanged:
                EventBus.Instance.OnSpawnPointsChanged += SetPoints;
                break;
            case ChangedEvent.OnSpawnPointsUnusedChanged:
                EventBus.Instance.OnSpawnPointsUnusedChanged += SetPoints;
                break;
        }
        EventBus.Instance.OnOutOfSpawnPoints += StartHighlightAnimation;
    }

    private void OnDisable()
    {
        switch (selectedEvent)
        {
            case ChangedEvent.OnSpawnPointsChanged:
                EventBus.Instance.OnSpawnPointsChanged -= SetPoints;
                break;
            case ChangedEvent.OnSpawnPointsUnusedChanged:
                EventBus.Instance.OnSpawnPointsUnusedChanged -= SetPoints;
                break;
        }
        EventBus.Instance.OnOutOfSpawnPoints -= StartHighlightAnimation;
    }

    private void SetPoints(int points, bool isReptilian)
    {
        if (this.isReptilian == isReptilian) slider.value = points;
    }

    private void StartHighlightAnimation(bool isReptilian)
    {
        if(this.isReptilian == isReptilian) StartCoroutine(HighlightAnimation());
    }

    private IEnumerator HighlightAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            Image frame = GetComponentInChildren<Image>();
            frame.sprite = sprites[0];
            yield return new WaitForSecondsRealtime(Constants.TIME_HIGHLIGHT_ANIMATION / 2);
            frame.sprite = sprites[1];
            yield return new WaitForSecondsRealtime(Constants.TIME_HIGHLIGHT_ANIMATION / 2);
        }
    }
}
