using UnityEngine;

public class ColliderDetection : MonoBehaviour
{
    [SerializeField] private float _targetingRange;
    [SerializeField] private float _targetingRadius = 1f;
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private int _circleSegments = 16; // smoothness of visualization

    private Vector3 _checkPoint;
    private Vector3 _forward;
    
    public Vector3 GetClosestCollider(Vector3 checkPoint, Vector3 forward)
    {
        _checkPoint = checkPoint;
        _forward = forward.normalized;

        DrawHitboxVisualization();
        
        Collider[] hits = Physics.OverlapSphere(
            checkPoint + forward * (_targetingRange * 0.5f),
            _targetingRadius,
            _collisionLayers
        );
        
        if (hits.Length == 0)
            return Vector3.zero;
        
        float minDistance = float.MaxValue;
        Collider closest = hits[0];

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(checkPoint, hit.bounds.center);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit;
            }
        }
        
        Debug.Log(closest);
        Debug.DrawLine(checkPoint, closest.bounds.center, Color.blue, 20f);

        return  closest.bounds.center;
    }
    
     private void DrawHitboxVisualization()
    {
        if (_forward == Vector3.zero) return;

        Vector3 start = _checkPoint;
        Vector3 end = _checkPoint + _forward * _targetingRange;

        // Create orthogonal axes
        Vector3 up = Vector3.up;
        Vector3 right = Vector3.Cross(_forward, up).normalized;
        up = Vector3.Cross(right, _forward).normalized;

        // Draw circles at start and end
        DrawCircle(start, right, up, _targetingRadius, Color.green);
        DrawCircle(end, right, up, _targetingRadius, Color.green);

        // Connect circles with lines
        for (int i = 0; i < _circleSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / _circleSegments;
            float nextAngle = ((i + 1) % _circleSegments) * Mathf.PI * 2f / _circleSegments;

            Vector3 startPoint  = start + right * Mathf.Cos(angle) * _targetingRadius + up * Mathf.Sin(angle) * _targetingRadius;
            Vector3 startNext   = start + right * Mathf.Cos(nextAngle) * _targetingRadius + up * Mathf.Sin(nextAngle) * _targetingRadius;

            Vector3 endPoint    = end + right * Mathf.Cos(angle) * _targetingRadius + up * Mathf.Sin(angle) * _targetingRadius;
            Vector3 endNext     = end + right * Mathf.Cos(nextAngle) * _targetingRadius + up * Mathf.Sin(nextAngle) * _targetingRadius;

            // Horizontal circle lines
            Debug.DrawLine(startPoint, startNext, Color.green, 20f);
            Debug.DrawLine(endPoint, endNext, Color.green, 20f);

            // Vertical connecting lines
            Debug.DrawLine(startPoint, endPoint, Color.yellow, 20f);
        }
    }

    private void DrawCircle(Vector3 center, Vector3 right, Vector3 up, float radius, Color color)
    {
        Vector3 previousPoint = center + right * radius;
        for (int i = 1; i <= _circleSegments; i++)
        {
            float angle = i * Mathf.PI * 2f / _circleSegments;
            Vector3 nextPoint = center + right * Mathf.Cos(angle) * radius + up * Mathf.Sin(angle) * radius;
            Debug.DrawLine(previousPoint, nextPoint, color, 20f);
            previousPoint = nextPoint;
        }
    }
}