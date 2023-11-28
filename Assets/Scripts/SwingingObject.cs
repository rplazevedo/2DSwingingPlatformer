using UnityEngine;

public class SwingingBlock : MonoBehaviour
{   
    private LineRenderer lineRenderer;
    private DistanceJoint2D distanceJoint;
    [SerializeField]
    private int vertexCount = 20;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = .1f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.red;
        lineRenderer.widthMultiplier = 0.2f;
        lineRenderer.useWorldSpace = true;

        distanceJoint = GetComponent<DistanceJoint2D>();
    }

    private void Update()
    {
        // TODO: make the rope connect the actual anchor point on the edge of the sprite
        var connectedAnchor = new Vector3 (distanceJoint.connectedAnchor.x, distanceJoint.connectedAnchor.y, 0);
        var anchor = new Vector3(distanceJoint.anchor.x, distanceJoint.anchor.y, 0);
        DrawLine(transform.position, connectedAnchor);
    }
    //For drawing line in the world space, provide the x,y,z values

    void DrawLine(Vector3 anchor, Vector3 connectedAnchor)
    {
        lineRenderer.SetPosition(0, new Vector3(anchor.x, anchor.y, 0));
        lineRenderer.SetPosition(1, new Vector3(connectedAnchor.x, connectedAnchor.y, 0));
    }
}
