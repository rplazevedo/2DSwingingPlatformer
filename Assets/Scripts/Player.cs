using Assets.Scripts.Input;
using UnityEngine;

public class Player : MonoBehaviour
{

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

    private GrapplingHook grapplingHook;

    private void Awake()
    {
        if (infiniteRange)
        {
            maxRange = 1000000;
        }
        grapplingHook = gameObject.AddComponent<GrapplingHook>();
        grapplingHook.Initialize(maxRange, minRange, grappleReelSpeed);

        startPosition = transform.position;
    }

    void Update()
    {
        if (!grapplingHook.IsGrappled())
        { 
            Move();
        }
        
        grapplingHook.Grapple(groundLayer);
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
        grapplingHook.DetachGrapple();
    }    
}
