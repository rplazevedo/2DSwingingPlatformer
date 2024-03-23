using Assets.Scripts.Input;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Range(1, 50)] private float maxGroundSpeed = 5;
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

    [Header("Grappling")]
    [SerializeField] private float maxRange = 50f;
    [SerializeField] private float minRange = 0f;
    [SerializeField] private bool infiniteRange = false;
    [SerializeField] private float grappleReelSpeed = 5f;
    [SerializeField] private float grappleCooldown = 1f;

    [Header("Power-ups")]
    [SerializeField] private int forwardBoostCount = 0;
    [SerializeField] private float forwardBoostStrength = 1f;
    [SerializeField] private float forwardBoostDuration = 0.5f;

    private Vector3 startPosition;
    private GrapplingHook grapplingHook;
    private bool _isBoosting;
    private float forwardBoostStartTime;



    public static Player instance;

    private void Awake()
    {
        instance = this;

        if (infiniteRange)
        {
            maxRange = 1000000;
        }

        grapplingHook = gameObject.AddComponent<GrapplingHook>();

        var grapppingHookProperties = new GrapplingHookProperties
        {
            MaxRange = maxRange,
            MinRange = minRange,
            Speed = grappleReelSpeed,
            GroundLayer = groundLayer,
            GrappleCooldown = grappleCooldown,
        };

        grapplingHook.Initialize(grapppingHookProperties);

        startPosition = transform.position;
    }

    private void Update()
    {
        if (!grapplingHook.IsGrappled())
        { 
            Move();
        }
        
        grapplingHook.Grapple();
        Boost();
    }

    private void Move()
    {
        var isGrounded = IsPlayerGrounded();

        ApplyCorrectFrictionMaterial(isGrounded);
        HorizontalMovement(isGrounded);
        
        if (ShouldJump(isGrounded))
        {
            Jump();
        }
    }

    private bool IsPlayerGrounded()
    {
        var collisionHit = Physics2D.BoxCast(
            playerCollider.bounds.center, 
            playerCollider.bounds.size, 
            0f, 
            Vector2.down, 
            0.1f, 
            groundLayer);

        return collisionHit.collider != null;
    }

    private void ApplyCorrectFrictionMaterial(bool isGrounded)
    {
        body.sharedMaterial = isGrounded ? highFrictionMaterial : lowFrictionMaterial;
    }

    private void HorizontalMovement(bool isGrounded)
    {
        if (!isGrounded && !allowAirMovement)
        {
            return;
        }

        var xForce = UserInput.GetHorizontalValue() * GetAcceleration(isGrounded);
        if (xForce == 0)
        {
            return;
        }

        var maxSpeed = GetMaxSpeed(isGrounded);
        if (HorizontalForceShouldBeApplied(xForce, maxSpeed))
        {
            var force = new Vector2(xForce, 0);

            body.AddForce(force, ForceMode2D.Force);
            return;
        }

        // This if statement smooths out movement on the ground, but prevents player from sliding if
        // holding same movement key as current direction. Haven't found another way to smooth movement though.
        if (isGrounded)
        {
            var newHorizontalVelocity = Mathf.Sign(body.velocity.x) * maxSpeed;
            body.velocity = new Vector2(newHorizontalVelocity, body.velocity.y);
        }
    }

    private float GetAcceleration(bool isGrounded)
    {
        return isGrounded ? groundAcceleration : airAcceleration;
    }

    private float GetMaxSpeed(bool isGrounded)
    {
        return isGrounded ? maxGroundSpeed : maxAirSpeed;
    }

    private bool HorizontalForceShouldBeApplied(float xForce, float maxSpeed)
    {
        var currentXVelocity = body.velocity.x;
        var isForceSpeedSameDirection = xForce * currentXVelocity > 0;
        var isSpeedOverMaxSpeed = Mathf.Abs(currentXVelocity) >= maxSpeed;

        return !isForceSpeedSameDirection || !isSpeedOverMaxSpeed;
    }

    private bool ShouldJump(bool isGrounded)
    {
        return UserInput.IsPressingJump() && isGrounded;
    }

    private void Jump()
    {
        body.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }

    private void Boost()
    {
        var isStartingBoost = forwardBoostCount > 0 && UserInput.GetRightMouseButtonDown();

        if (!_isBoosting && !isStartingBoost)
        {
            return;
        }

        if (!_isBoosting && isStartingBoost)
        {
            ActivateBoost();
        }

        _isBoosting = Time.time - forwardBoostStartTime <= forwardBoostDuration;

        if (_isBoosting)
        {
            ApplyBoost();
        }
    }

    private void ActivateBoost()
    {
        forwardBoostStartTime = Time.time;
        forwardBoostCount--;
        GameUI.instance.UpdateBoostCount(forwardBoostCount);
    }

    private void ApplyBoost()
    {
        var currentDirection = body.velocity.normalized;
        var boostForce = new Vector2(currentDirection.x, currentDirection.y) * forwardBoostStrength;
        body.AddForce(boostForce, ForceMode2D.Force);
    }

    public void AddForwardBoost()
    {
        forwardBoostCount++;
        GameUI.instance.UpdateBoostCount(forwardBoostCount);
    }

    internal void Reset()
    {
        transform.position = startPosition;
        body.velocity = Vector2.zero;
        grapplingHook.DetachGrapple();
    }    
}
