using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset;

    private float updateDelay = 0.25f;
    private float xOffset = 2f;
    private Vector3 velocity;
    private Vector3 offset;

    private void Awake()
    {
        velocity = Vector3.zero;
        offset = new Vector3(xOffset, yOffset, -10);
    }

    private void Update()
    {
        xOffset = body.velocity.x / 2;
        offset = new Vector3(xOffset, yOffset, -10);
        var targetPosition = target.position + offset;
        transform.position = GetNewPosition(targetPosition);
    }

    private Vector3 GetNewPosition(Vector3 targetPosition)
    {
        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, updateDelay);
    }
}
