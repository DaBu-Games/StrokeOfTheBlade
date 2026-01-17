using UnityEngine;

public class ColliderDetection : MonoBehaviour
{
    [SerializeField] private float _targetingRange;
    [SerializeField] private float _targetingRadius = 1f;
    [SerializeField] private LayerMask _collisionLayers;
    
    public Transform GetClosestCollider(Vector3 checkPoint, Vector3 forward)
    {
        Collider[] hits = Physics.OverlapSphere(
            checkPoint + forward * (_targetingRange * 0.5f),
            _targetingRadius,
            _collisionLayers
        );
        
        if (hits.Length == 0)
            return null;
        
        float minDistance = float.MaxValue;
        Collider closest = hits[0];

        foreach (var hit in hits)
        {
            float dist = Vector3.Distance(checkPoint, hit.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = hit;
            }
        }

        return  closest.transform;
    }
}