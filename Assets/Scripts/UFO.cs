using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Instance.OnUFO += StartUFOAnimation;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnUFO -= StartUFOAnimation;
    }

    private IEnumerator UFOAnimation(Vector3 position, float duration)
    {
        float time = 0;
        Vector3 startPosition = transform.position;

        while (time < duration / 2)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.position = Vector3.Lerp(startPosition, position, normalizedTime);
            yield return null;
        }
        transform.position = position;

        yield return new WaitForSeconds(Constants.TIME_ROTATE3D_ANIMATION);

        time = 0;
        while (time < duration / 2)
        {
            time += Time.deltaTime;
            float normalizedTime = time / duration;
            transform.position = Vector3.Lerp(position, new Vector3(-startPosition.x, startPosition.y, startPosition.z), normalizedTime);
            yield return null;
        }
        transform.position = startPosition;
    }

    private void StartUFOAnimation(Vector3 position)
    {
        StartCoroutine(UFOAnimation(position, Constants.TIME_UFO_ANIMATION));
    }
}
