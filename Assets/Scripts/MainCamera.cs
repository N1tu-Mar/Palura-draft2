using UnityEngine;

public class PerfectSmoothCameraFollow2D : MonoBehaviour
{
    public Transform target;

    [Header("Smooth Follow")]
    public float followSmoothTime = 0.05f;   // small value = fast response
    private Vector3 followVelocity = Vector3.zero;

    [Header("Look Ahead")]
    public float lookAheadX = 0.4f;
    public float lookAheadY = 0.25f;
    public float lookAheadSmooth = 0.15f;

    private Rigidbody2D rb;
    private Vector2 targetLookAhead;
    private Vector2 currentLookAhead;

    void Start()
    {
        if (target != null)
            rb = target.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        Vector2 vel = rb.linearVelocity;

        targetLookAhead = Vector2.zero;

        if (Mathf.Abs(vel.x) > 0.05f)
            targetLookAhead.x = Mathf.Sign(vel.x) * lookAheadX;

        if (Mathf.Abs(vel.y) > 0.05f)
            targetLookAhead.y = Mathf.Sign(vel.y) * lookAheadY;

        currentLookAhead = Vector2.Lerp(currentLookAhead, targetLookAhead, Time.deltaTime * (1f / lookAheadSmooth));
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPos = new Vector3(
            target.position.x + currentLookAhead.x,
            target.position.y + currentLookAhead.y,
            transform.position.z
        );

        // SmoothDamp with a LOW smoothTime = fast, accurate camera
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPos,
            ref followVelocity,
            followSmoothTime
        );
    }
}
