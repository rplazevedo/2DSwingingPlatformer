using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class GrappleProperties : MonoBehaviour
    {
        [SerializeField] public bool grappleable;
        [SerializeField] public bool swingable;

        private List<Vector2> swingablePoints;
        private Bounds bounds;

        private void Start()
        {
            var collider = GetComponent<Collider2D>();
            bounds = collider.bounds;
            
            swingablePoints = new List<Vector2>
            {
                new(bounds.min.x, bounds.min.y),
                new(bounds.min.x, bounds.max.y),
                new(bounds.max.x, bounds.min.y),
                new(bounds.max.x, bounds.max.y)
            };
        }

        internal Vector2 GetClosestCorner(Vector2 point)
        {
            var minDistance = float.MaxValue;
            var closestCorner = Vector2.zero;

            foreach (Vector2 corner in swingablePoints)
            {
                var distance = Vector2.Distance(point, corner);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestCorner = corner;
                }
            }

            return closestCorner;
        }
    }
}
