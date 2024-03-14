using UnityEngine;
using UnityEngine.UI;

public class BossAvatar : MonoBehaviour
{
    [SerializeField] private bool isReptilian;
    private static Sprite[] avatars;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        avatars ??= Resources.LoadAll<Sprite>("bosses");
    }

    private void OnEnable()
    {
        EventBus.Instance.OnBossAvatarChanged += ChangeAvatar;
    }

    private void OnDisable()
    {
        EventBus.Instance.OnBossAvatarChanged -= ChangeAvatar;
    }

    private void ChangeAvatar(int avatarNumber, bool isReptilian)
    {
        if (this.isReptilian == isReptilian) image.sprite = avatars[avatarNumber];
    }
}
