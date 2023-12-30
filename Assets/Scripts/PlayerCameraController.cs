using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset;

    private float updateDelay = 0.25f;
    private Vector3 velocity;

    private void Awake()
    {
        velocity = Vector3.zero;
    }

    private void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.position = GetNewPosition();
    }

    private Vector3 GetNewPosition()
    {
        var offset = new Vector3(body.velocity.x / 2, yOffset, -10);
        var targetPosition = target.position + offset;

        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, updateDelay);
    }
}
