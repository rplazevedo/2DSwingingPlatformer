using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float yOffset;

    private float updateDelay = 0.25f;
    private Vector3 velocity;
    private Rigidbody2D playerBody;
    private Transform playerTransform;


    private void Awake()
    {
        velocity = Vector3.zero;
        playerBody = player.GetComponent<Rigidbody2D>();
        playerTransform = player.GetComponent<Transform>();
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
        var offset = new Vector3(playerBody.velocity.x / 2, yOffset, -10);
        var targetPosition = playerTransform.position + offset;

        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, updateDelay);
    }
}
