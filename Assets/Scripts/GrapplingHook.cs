using Assets.Scripts;
using Assets.Scripts.Input;
using UnityEngine;

public class GrapplingHookProperties
{
    public float MaxRange { get; internal set; }
    public float MinRange { get; internal set; }
    public float Speed { get; internal set; }
    public LayerMask GroundLayer { get; internal set; }
}

public class GrapplingHook : MonoBehaviour
{
    private Camera cam;

    private bool _isGrappled;
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;
    private LayerMask groundLayer;
    private float maxRange = 50f;
    private float minRange = 0f;
    private float grappleReelSpeed = 5f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        distanceJoint = GetComponent<DistanceJoint2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        cam = Camera.main;
    }

    internal void Initialize(GrapplingHookProperties grapppingHookProperties)
    {
        maxRange = grapppingHookProperties.MaxRange;
        minRange = grapppingHookProperties.MinRange;
        grappleReelSpeed = grapppingHookProperties.Speed;
        groundLayer = grapppingHookProperties.GroundLayer;
    }

    internal void Grapple()
    {
        if (ShouldFireGrapple())
        {
            FireGrapple();
            return;
        }

        if (IsGrappled())
        {
            UpdateGrapplingHook();
        }
    }

    private bool ShouldFireGrapple()
    {
        return !IsGrappled() && UserInput.GetLeftMouseButtonDown();
    }

    private void FireGrapple()
    {
        var mouseCoord = cam.ScreenToWorldPoint(Input.mousePosition);
        var linecastEnd = (mouseCoord - transform.position) * maxRange;
        var hit = CheckForHit(transform.position, linecastEnd);

        if (HitGrappleableComponent(hit))
        {
            distanceJoint.connectedAnchor = hit.point;
            distanceJoint.enabled = true;
            lineRenderer.enabled = true;
            _isGrappled = true;
        }
    }

    private RaycastHit2D CheckForHit(Vector3 linecastStart, Vector3 linecastEnd)
    {
        return Physics2D.Linecast(linecastStart, linecastEnd, groundLayer);
    }

    private static bool HitGrappleableComponent(RaycastHit2D hit)
    {
        if (hit.collider == null)
        {
            return false;
        }
        var hasGrappleProperties = hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);
        return hit && hasGrappleProperties && grappleProperties.grappleable;
    }

    private void UpdateGrapplingHook()
    {
        DetectGrappleLineCollision();
        DrawLine();
        ReelGrapple(); 
        
        if (UserInput.GetLeftMouseButtonDown())
        {
            DetachGrapple();
        }
    }

    private void DetectGrappleLineCollision()
    {
        var world_anchor = (Vector2)transform.TransformPoint(distanceJoint.anchor);

        var connected_anchor = distanceJoint.connectedAnchor;

        var direction = (connected_anchor - world_anchor).normalized;
        var linecastEnd = connected_anchor - (direction * 0.1f);
        var hit = CheckForHit(world_anchor, linecastEnd);

        if (hit && hit.collider.gameObject != gameObject && HitSwingableComponent(hit))
        {
            hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);

            var closestPointOnPerimeter = grappleProperties.GetClosestCorner(hit.point);
            distanceJoint.connectedAnchor = closestPointOnPerimeter;
        }
    }

    private static bool HitSwingableComponent(RaycastHit2D hit)
    {
        if (hit.collider == null)
        {
            return false;
        }
        var hasGrappleProperties = hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);
        return hit && hasGrappleProperties && grappleProperties.swingable;
    }

    private void DrawLine()
    {
        var world_anchor = transform.TransformPoint(distanceJoint.anchor);
        lineRenderer.SetPosition(0, world_anchor);
        lineRenderer.SetPosition(1, distanceJoint.connectedAnchor);
    }

    private void ReelGrapple()
    {
        var grappleVerticalSpeed = UserInput.GetVerticalValue() * grappleReelSpeed;
        if (CanReelGrapple(grappleVerticalSpeed))
        {
            distanceJoint.distance -= grappleVerticalSpeed * Time.deltaTime;
            ResolveGrappleCollisions();
        }
    }

    private bool CanReelGrapple(float grappleVerticalSpeed)
    {
        bool canReelIn = grappleVerticalSpeed > 0 && distanceJoint.distance > minRange;
        bool canReelOut = grappleVerticalSpeed < 0 && distanceJoint.distance < maxRange;

        return canReelIn || canReelOut;
    }

    private void ResolveGrappleCollisions()
    {
        var hits = Physics2D.OverlapBoxAll(transform.position, boxCollider.size, 0);
        foreach (var hit in hits)
        {
            if (hit == boxCollider)
            {
                continue;
            }

            var colliderDistance = hit.Distance(boxCollider);
            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }
        }
    }

    public void DetachGrapple()
    {
        distanceJoint.enabled = false;
        lineRenderer.enabled = false;
        _isGrappled = false;
    }

    internal bool IsGrappled()
    {
        return _isGrappled;
    }
}