using Assets.Scripts;
using Assets.Scripts.Input;
using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
    private Camera cam;

    private bool _isGrappled;
    private DistanceJoint2D distanceJoint;
    private LineRenderer lineRenderer;
    private BoxCollider2D boxCollider;


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

    internal void Initialize(float maxRange, float minRange, float grappleReelSpeed)
    {
        this.maxRange = maxRange;
        this.minRange = minRange;
        this.grappleReelSpeed = grappleReelSpeed;
    }

    internal void Grapple(LayerMask groundLayer)
    {
        if (CanGrapple() && UserInput.GetLeftMouseButtonDown())
        {
            FireGrapple(groundLayer);
        }
        else if (_isGrappled)
        {
            UpdateGrapplingHook(groundLayer);
        }
    }

    private bool CanGrapple()
    {
        return !_isGrappled;
    }

    private void FireGrapple(LayerMask groundLayer)
    {
        var mouseCoord = cam.ScreenToWorldPoint(Input.mousePosition);
        var hit = Physics2D.Linecast(transform.position, (mouseCoord - transform.position) * maxRange, groundLayer);

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
        if (hit.collider == null)
        {
            return false;
        }
        var hasGrappleProperties = hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);
        return hit && hasGrappleProperties && grappleProperties.grappleable;
    }

    private void UpdateGrapplingHook(LayerMask groundLayer)
    {
        DetectGrappleLineCollision(groundLayer);
        DrawLine();
        ReelGrapple();
        DetachGrappleOnClick();
    }

    private void DetectGrappleLineCollision(LayerMask groundLayer)
    {
        var world_anchor = (Vector2)transform.TransformPoint(distanceJoint.anchor);

        var connected_anchor = distanceJoint.connectedAnchor;

        Vector2 direction = (connected_anchor - world_anchor).normalized;
        var hit = Physics2D.Linecast(world_anchor, connected_anchor - (direction * 0.1f), groundLayer);

        if (hit && hit.collider.gameObject != gameObject && HitSwingableComponent(ref hit))
        {
            hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);

            var closestPointOnPerimeter = grappleProperties.GetClosestCorner(hit.point);
            distanceJoint.connectedAnchor = closestPointOnPerimeter;
        }
    }

    private static bool HitSwingableComponent(ref RaycastHit2D hit)
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

    internal bool IsGrappled()
    {
        return _isGrappled;
    }
}