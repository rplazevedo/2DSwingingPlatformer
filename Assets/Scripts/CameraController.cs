using Assets.Scripts.Input;
using UnityEngine;

public class CameraController : MonoBehaviour
{
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
        //If Player isMovingLeft then set xOffset to -2
        offset = new Vector3(xOffset, yOffset, -10);
        if (UserInput.GetHorizontalValue() < 0) //TODO Review this - maybe we want to take player's velocity instead
        {
            offset.x = -xOffset;
        }
        var targetPosition = target.position + offset;
        transform.position = GetNewPosition(targetPosition);
    }

    private Vector3 GetNewPosition(Vector3 targetPosition)
    {
        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, updateDelay);
    }
}
