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
    [SerializeField] private float maxRange = 50f;
    [SerializeField] private float minRange = 0f;
    [SerializeField] private bool infiniteRange = false;
    [SerializeField] private float grappleReelSpeed = 5f;

    private bool _isGrappled;
    private LineRenderer lineRenderer;
    private DistanceJoint2D distanceJoint;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        boxCollider = GetComponent<BoxCollider2D>(); 

        if (infiniteRange)
        {
            maxRange = 1000000;
        }
        _isGrappled = false;
        cam = Camera.main;
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
        var currentXSpeed = body.velocity.x;
        if (xSpeed > 0)
        {
            body.velocity = new Vector2(Mathf.Max(xSpeed, currentXSpeed), body.velocity.y);
        }
        else if (xSpeed < 0) 
        {
            body.velocity = new Vector2(Mathf.Min(xSpeed, currentXSpeed), body.velocity.y);
        }

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
        DetachGrapple();
    }

    private void Grapple()
    {   
        if (CanGrapple() && UserInput.GetLeftMouseButtonDown())
        {
            FireGrapple();
        }
        else if (_isGrappled)
        {
            DrawLine();
            ReelGrapple();
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
        if (hit && hit.collider.gameObject.layer == 6) // This is not very explicit that layer 6 is the Ground layer
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




    private void ReelGrapple()
    {
        var grappleVerticalSpeed = UserInput.GetVerticalValue() * grappleReelSpeed;
        if (CanReelGrapple(grappleVerticalSpeed))
        {
            distanceJoint.distance -= grappleVerticalSpeed * Time.deltaTime;
            SolveGrappleCollisions();
        }
    }

    private bool CanReelGrapple(float grappleVerticalSpeed)
    {
        bool canReelIn = grappleVerticalSpeed > 0 && distanceJoint.distance > minRange;
        bool canReelOut = grappleVerticalSpeed < 0 && distanceJoint.distance < maxRange;
        return canReelIn || canReelOut;
    }

    private void SolveGrappleCollisions()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        foreach (Collider2D hit in hits)
        {
            if (hit == boxCollider)
                continue;
            ColliderDistance2D colliderDistance = hit.Distance(boxCollider);
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }
        }

    }

    private void DetachGrappleOnClick()
    {
        if (UserInput.GetLeftMouseButtonDown())
        {
            DetachGrapple();
        }
    }

    public void DetachGrapple()
    {
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        _isGrappled = false;
    }
}