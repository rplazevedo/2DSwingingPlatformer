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
    private Vector3 velocity3;
    private float velocity1;
    private Rigidbody2D playerBody;
    private Transform playerTransform;


    private void Awake()
    {
        velocity3 = Vector3.zero;
        velocity1 = 0f;
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

        return Vector3.SmoothDamp(transform.position, targetPosition, ref velocity3, updateDelay);
    }

    private float GetNewSize()
    {   
        var targetSize = minSize + (playerBody.velocity.magnitude - minSizeSpeed) / (maxSizeSpeed - minSizeSpeed) * (maxSize - minSize);
        var clampedSize = Mathf.Clamp(targetSize, minSize, maxSize);
        return Mathf.SmoothDamp(Camera.main.orthographicSize, clampedSize, ref velocity1, updateDelay);
    }
}
