using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public static Draggable draggedNow;
    protected GameObject target;
    protected bool isLocked = false;
    protected bool isPlaced = false;
    protected Vector3 startPosition;
    private Camera mainCamera;
    private int collisionCounter = 0;

    [SerializeField] private string targetTag;
    [SerializeField] private bool lockWhenPlaced;

    protected virtual void OnAwake() { }
    private void Awake()
    {
        mainCamera = Camera.main;
        OnAwake();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        draggedNow = this;
        isPlaced = false;
        startPosition = transform.position;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (isLocked) return;
        transform.position = mainCamera.ScreenToWorldPoint(
            new Vector3(
                eventData.position.x,
                eventData.position.y,
                -mainCamera.transform.position.z));
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        draggedNow = null;
        if (target != null && CanBePlaced())
        {
            transform.position = target.transform.position;
            isLocked = lockWhenPlaced;
            isPlaced = true;
            startPosition = transform.position;
        }
        else
        {
            ToStartPosition();
        }
    }

    public void ToStartPosition()
    {
        transform.position = startPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(targetTag)) return;
        collisionCounter++;
        target = other.gameObject;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag(targetTag)) return;
        collisionCounter--;
        if (collisionCounter == 0)
        {
            target = null;
        }
    }
    protected virtual bool CanBePlaced()
    {
        return true;
    }

    public void Lock()
    {
        isLocked = true;
    }

    public void Unlock()
    {
        isLocked = false;
    }
}
