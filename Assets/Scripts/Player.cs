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

    [Header("Grappling")]
    [SerializeField] float maxRange = 50f;
    [SerializeField] bool infiniteRange = false;
    private bool _isGrappled;
    private LineRenderer lineRenderer;
    private DistanceJoint2D distanceJoint;

    private void Awake()
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

        distanceJoint = GetComponent<DistanceJoint2D>();
        distanceJoint.anchor = Vector2.zero;
        distanceJoint.enabled = false;

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

    private void Grapple()
    {   
        // TODO: Refactor component functions
        if (CanGrapple())
        {
            FireGrappleOnClick();
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

    private void FireGrappleOnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FireGrapple();
        }
    }

    private void FireGrapple()
    {   
        var mouseCoord = cam.ScreenToWorldPoint(Input.mousePosition);
        if (infiniteRange)
        {
            maxRange = 1000000;
        }
        RaycastHit2D hit = Physics2D.Linecast(transform.position, (mouseCoord - transform.position) * maxRange);
        if (hit)
        {
            // TODO?: Incorporate target physics
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
            distanceJoint.connectedAnchor = hit.point;
            _isGrappled = true;
        }
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

    void DrawLine()
    {
        var connectedAnchor = new Vector3(distanceJoint.connectedAnchor.x, distanceJoint.connectedAnchor.y, 0);
        var anchor = new Vector3(distanceJoint.anchor.x, distanceJoint.anchor.y, 0);
        var world_anchor = transform.TransformPoint(anchor);
        lineRenderer.SetPosition(0, new Vector3(world_anchor.x, world_anchor.y, 0));
        lineRenderer.SetPosition(1, new Vector3(connectedAnchor.x, connectedAnchor.y, 0));
    }

}