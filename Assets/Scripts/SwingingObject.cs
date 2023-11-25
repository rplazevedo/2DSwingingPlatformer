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
        var connectedAnchor = distanceJoint.connectedAnchor;
        var anchor = distanceJoint.anchor;
        DrawLine();
    }
    //For drawing line in the world space, provide the x,y,z values

    void DrawLine()
    {
        lineRenderer.positionCount = vertexCount + 1;

        Vector3[] positions = new Vector3[vertexCount + 1];

        for (int i = 0; i <= vertexCount; i++)
        {
            float t = i / (float)vertexCount;
            positions[i] = Vector3.Lerp(distanceJoint.connectedBody.transform.position, transform.position, t);
        }

        lineRenderer.SetPositions(positions);
    }
}
