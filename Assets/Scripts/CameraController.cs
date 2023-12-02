using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float yOffset;

    private float updateDelay = 0.25f;
    private Vector3 velocity;
    private Vector3 offset;

    private void Awake()
    {
        velocity = Vector3.zero;
        offset = new Vector3(0, yOffset, -10);
    }

    private void Update()
    {
        var targetPosition = target.position + offset;
        transform.position = GetNewPosition(targetPosition);
    }

    private Vector3 GetNewPosition(Vector3 targetPosition)
    {
        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, updateDelay);
    }
}
