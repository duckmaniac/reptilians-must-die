using UnityEngine;

public class SpaceTraveler : MonoBehaviour
{
    private Vector2 movementDirection;
    private float speed = 0.04f;
    private float rotationSpeed = 0.6f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SetRandomDirection();
    }

    void Update()
    {
        Move();
        CheckBoundsAndTeleport();
    }

    private void SetRandomDirection()
    {
        float angle = Random.Range(0, 360) * Mathf.Deg2Rad;
        movementDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb.velocity = movementDirection * speed;
    }

    private void Move()
    {
        rb.velocity = movementDirection * speed;
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
    }

    private void CheckBoundsAndTeleport()
    {
        var viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        bool isOutside = false;

        if (viewportPosition.x > 1)
        {
            viewportPosition.x = 0;
            isOutside = true;
        }
        else if (viewportPosition.x < 0)
        {
            viewportPosition.x = 1;
            isOutside = true;
        }

        if (viewportPosition.y > 1)
        {
            viewportPosition.y = 0;
            isOutside = true;
        }
        else if (viewportPosition.y < 0)
        {
            viewportPosition.y = 1;
            isOutside = true;
        }

        if (isOutside)
        {
            transform.position = Camera.main.ViewportToWorldPoint(viewportPosition);
            SetRandomDirection();
        }
    }
}
