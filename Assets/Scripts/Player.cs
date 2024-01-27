using Assets.Scripts;
using Assets.Scripts.Input;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Camera cam;

    [Header("Movement")]
    [SerializeField] private float maxGroundSpeed = 5;
    [SerializeField] private float groundAcceleration = 5;
    [SerializeField] private bool allowAirMovement = true;
    [SerializeField] private float maxAirSpeed = 5;
    [SerializeField] private float airAcceleration = 0.5f;
    [SerializeField] private float jumpForce = 10;
    [SerializeField] private PhysicsMaterial2D highFrictionMaterial;
    [SerializeField] private PhysicsMaterial2D lowFrictionMaterial;

    [Header("Collision")]
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private LayerMask groundLayer;
    private Vector3 startPosition;

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
        startPosition = transform.position;
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
        var isGrounded = IsGrounded();

        HorizontalMovement(isGrounded);

        AdjustFriction(isGrounded);

        if (ShouldJump(isGrounded))
        {
            Jump();
        }
    }

    private void HorizontalMovement(bool isGrounded)
    {
        float acceleration;
        float maxSpeed;
        if (isGrounded)
        {
            maxSpeed = maxGroundSpeed;
            acceleration = groundAcceleration;
        }
        else if (allowAirMovement)
        {
            maxSpeed = maxAirSpeed;
            acceleration = airAcceleration;
        }
        else { return; }

        var xForce = UserInput.GetHorizontalValue() * acceleration;
        var currentXVelocity = body.velocity.x;

        var isForceSpeedSameDirection = xForce * currentXVelocity > 0;
        var isSpeedOverMaxSpeed = Mathf.Abs(currentXVelocity) >= maxSpeed;

        if (xForce == 0 )
        {
            return;
        }

        if( isForceSpeedSameDirection && isSpeedOverMaxSpeed)
        {   
            // This if statement smooths out movement on the ground, but prevents player from sliding if
            // holding same movement key as current direction. Haven't found another way to smooth movement though.
            if (isGrounded)
            {
                body.velocity = new Vector2(Mathf.Sign(currentXVelocity) * maxSpeed, body.velocity.y);
            }
            return;
        }

        var force = new Vector2(xForce, 0);

        body.AddForce(force, ForceMode2D.Force);
    }

    private void AdjustFriction(bool isGrounded)
    {
        body.sharedMaterial = isGrounded ? highFrictionMaterial : lowFrictionMaterial;
    }

    private bool ShouldJump(bool isGrounded)
    {
        return UserInput.IsPressingJump() && isGrounded;
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
        body.velocity = Vector2.zero;
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
            DetectGrappleLineCollision();
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
        var hit = Physics2D.Linecast(transform.position, (mouseCoord - transform.position) * maxRange);

        if (HitGrappleableComponent(ref hit))
        {
            distanceJoint.connectedAnchor = hit.point;
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
            _isGrappled = true;
        }
    }

    private static bool HitGrappleableComponent(ref RaycastHit2D hit)
    {
        return hit && hit.collider.gameObject.TryGetComponent<Grappleable>(out _);
    }

    private void DetectGrappleLineCollision()
    {
        var connectedAnchor = new Vector3(distanceJoint.connectedAnchor.x, distanceJoint.connectedAnchor.y, 0);
        var anchor = new Vector3(distanceJoint.anchor.x, distanceJoint.anchor.y, 0);
        var world_anchor = transform.TransformPoint(anchor);
        var hit = Physics2D.Linecast(connectedAnchor, world_anchor);

        if (HitGrappleableComponent(ref hit))
        {
            var hitPoint = hit.point;
            Debug.Log(hitPoint);
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
        foreach (var hit in hits)
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