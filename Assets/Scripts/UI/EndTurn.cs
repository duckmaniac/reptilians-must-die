using UnityEngine;

public class EndTurn : MonoBehaviour
{
    public void OnClick()
    {
        EventBus.Instance.RaiseOnTurnEnded();
    }
}
