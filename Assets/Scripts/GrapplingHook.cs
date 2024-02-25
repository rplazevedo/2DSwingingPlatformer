using Assets.Scripts;
using Assets.Scripts.Extensions;
using Assets.Scripts.Input;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private List<Vector3> connectedPoints;
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
        connectedPoints = new List<Vector3>();

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
            ConnectGrapple(hit);
        }
    }

    private RaycastHit2D CheckForHit(Vector3 linecastStart, Vector3 linecastEnd)
    {
        return Physics2D.Linecast(linecastStart, linecastEnd, groundLayer);
    }

    private bool HitGrappleableComponent(RaycastHit2D hit)
    {
        if (!hit)
        {
            return false;
        }

        var hasGrappleProperties = hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);
        return hasGrappleProperties && grappleProperties.grappleable;
    }

    private void ConnectGrapple(RaycastHit2D hit)
    {
        distanceJoint.connectedAnchor = hit.point;
        distanceJoint.enabled = true;
        lineRenderer.enabled = true;

        var playerAnchor = transform.TransformPoint2D(distanceJoint.anchor);

        connectedPoints.Add(playerAnchor);
        connectedPoints.Add(hit.point);

        _isGrappled = true;
    }

    private void UpdateGrapplingHook()
    {
        CheckAndHandleWrapping();
        CheckAndHandleUnwrapping();
        DrawLine();
        ReelGrapple(); 
        
        if (UserInput.GetLeftMouseButtonDown())
        {
            DetachGrapple();
        }
    }

    private void CheckAndHandleWrapping()
    {
        var playerAnchor = transform.TransformPoint2D(distanceJoint.anchor);

        var connectedAnchor = distanceJoint.connectedAnchor;

        var direction = (connectedAnchor - playerAnchor).normalized;
        var linecastEnd = connectedAnchor - (direction * 0.1f);
        var hit = CheckForHit(playerAnchor, linecastEnd);

        if (HitSwingableComponent(hit))
        {
            hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);

            var closestPointOnPerimeter = grappleProperties.GetClosestCorner(hit.point);
            distanceJoint.connectedAnchor = closestPointOnPerimeter;
            connectedPoints.Insert(1, distanceJoint.connectedAnchor);
        }
    }

    private bool HitSwingableComponent(RaycastHit2D hit)
    {
        if(!hit || hit.collider.gameObject == gameObject)
        {
            return false;
        }

        var hasGrappleProperties = hit.collider.TryGetComponent<GrappleProperties>(out var grappleProperties);
        return hasGrappleProperties && grappleProperties.swingable;
    }

    private void CheckAndHandleUnwrapping()
    {
        if (connectedPoints.Count <= 2)
        {
            return;
        }

        var lastPoint = (Vector2)connectedPoints[2];

        var playerAnchor = transform.TransformPoint2D(distanceJoint.anchor);
        var currentDirection = distanceJoint.connectedAnchor - playerAnchor;
        var lastDirection = (lastPoint - playerAnchor).normalized;
        var linecastEnd = lastPoint - (lastDirection * 0.1f);
        var hit = CheckForHit(playerAnchor, linecastEnd);

        var angle = Vector3.Angle(currentDirection, lastDirection);
        var isSameAngle = angle <= 1f;

        if (!hit && isSameAngle)
        {
            distanceJoint.connectedAnchor = lastPoint;
            connectedPoints.RemoveAt(1);
        }
    }

    private void DrawLine()
    {
        var world_anchor = transform.TransformPoint(distanceJoint.anchor);
        connectedPoints[0] = world_anchor;
        lineRenderer.UpdateLinePoints(connectedPoints);
    }

    private void ReelGrapple()
    {
        var currentReelSpeed = UserInput.GetVerticalValue() * grappleReelSpeed;
        if (CanReelGrapple(currentReelSpeed))
        {
            distanceJoint.distance -= currentReelSpeed * Time.deltaTime;
            ResolveGrappleCollisions();
        }
    }

    private bool CanReelGrapple(float currentReelSpeed)
    {
        var canReelIn = currentReelSpeed > 0 && distanceJoint.distance > minRange;
        var canReelOut = currentReelSpeed < 0 && distanceJoint.distance < maxRange;

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
        connectedPoints.Clear();
        lineRenderer.UpdateLinePoints(connectedPoints);
    }

    internal bool IsGrappled()
    {
        return _isGrappled;
    }
}