using Assets.Scripts.Input;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera cam;

    [Header("Movement")]
    [SerializeField] private float speed = 5;
    [SerializeField] private float jumpForce = 10;

    [Header("Collision")]
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    [Header("Grappling")]
    [SerializeField] float maxRange = 50f;
    [SerializeField] bool infiniteRange = false;
    private bool _isGrappled;
    private LineRenderer lineRenderer;
    private DistanceJoint2D distanceJoint;

    private void Awake()
    {
        SetupLineRenderer();
        SetupDistanceJoint();

        if (infiniteRange)
        {
            maxRange = 1000000;
        }
        _isGrappled = false;
        cam = Camera.main;
    }

    private void SetupLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = .02f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
    }

    private void SetupDistanceJoint()
    {
        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.anchor = Vector2.zero;
        distanceJoint.enabled = false;
        distanceJoint.maxDistanceOnly = true;
    }

    void Update()
    {
        if (!_isGrappled)
        { 
            Move();
        }
        
        Grapple();
    }

    private void Move()
    {   
        HorizontalMovement();

        if (ShouldJump())
        {
            Jump();
        }
    }

    private void HorizontalMovement()
    {
        var xSpeed = UserInput.GetHorizontalValue() * speed;
        body.velocity = new Vector2(xSpeed, body.velocity.y);
    }

    private bool ShouldJump()
    {
        return UserInput.IsPressingJump() && IsGrounded();
    }

    private bool IsGrounded()
    {
        var collisionHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, 0.1f, groundLayer);

        return collisionHit.collider != null;
    }

    private void Jump()
    {
        body.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    internal void Reset()
    {
        transform.position = startPosition;
    }

    private void Grapple()
    {   
        if (CanGrapple() && Input.GetMouseButtonDown(0))
        {
            FireGrapple();
        }
        else if (_isGrappled)
        {
            DrawLine();
            DetachGrappleOnClick();        
        }
    }

    private bool CanGrapple()
    {
        return !_isGrappled;
    }

    private void FireGrapple()
    {   
        var mouseCoord = cam.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Linecast(transform.position, (mouseCoord - transform.position) * maxRange);
        if (hit)
        {
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
            distanceJoint.connectedAnchor = hit.point;
            _isGrappled = true;
        }
    }

    private void DrawLine()
    {
        var connectedAnchor = new Vector3(distanceJoint.connectedAnchor.x, distanceJoint.connectedAnchor.y, 0);
        var anchor = new Vector3(distanceJoint.anchor.x, distanceJoint.anchor.y, 0);
        var world_anchor = transform.TransformPoint(anchor);
        lineRenderer.SetPosition(0, world_anchor);
        lineRenderer.SetPosition(1, connectedAnchor);
    }

    private void DetachGrappleOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            distanceJoint.enabled = false;
            lineRenderer.enabled = false;
            _isGrappled = false;
        }
    }
}