using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float yOffset;
    [SerializeField] private float minSize = 10f;
    [SerializeField] private float maxSize = 20f;
    [SerializeField] private float maxSizeSpeed = 50f;
    [SerializeField] private float minSizeSpeed = 5f;

    private float updateDelay = 0.25f;
    private Vector3 positionVelocity;
    private float resizeVelocity;
    private Rigidbody2D playerBody;
    private Transform playerTransform;


    private void Awake()
    {
        positionVelocity = Vector3.zero;
        resizeVelocity = 0f;
        playerBody = player.GetComponent<Rigidbody2D>();
        playerTransform = player.GetComponent<Transform>();
    }

    private void Update()
    {
        UpdatePosition();
        UpdateSize();
    }
    
    private void UpdatePosition()
    {
        transform.position = GetNewPosition();
    }

    private void UpdateSize()
    {
        Camera.main.orthographicSize = GetNewSize();
    }
    
    private Vector3 GetNewPosition()
    {
        var offset = new Vector3(playerBody.velocity.x / 2, yOffset, -10);
        var targetPosition = playerTransform.position + offset;

        return Vector3.SmoothDamp(transform.position, targetPosition, ref positionVelocity, updateDelay);
    }

    private float GetNewSize()
    {
        var speedOverMinimumThreshold = playerBody.velocity.magnitude - minSizeSpeed;
        var speedRange = maxSizeSpeed - minSizeSpeed;

        var speedMultiplicationFactor = speedOverMinimumThreshold / speedRange;
        var sizeMultiplier = Mathf.Clamp(speedMultiplicationFactor, 0, 1);

        var sizeRange = maxSize - minSize;
        var targetSize = (sizeMultiplier * sizeRange) + minSize;
        
        return Mathf.SmoothDamp(Camera.main.orthographicSize, targetSize, ref resizeVelocity, updateDelay);
    }
}
