using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Katana : BaseController
{
    [Header("katana value's")]
    [SerializeField] private Transform _tip;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _elementPrefab;
    [SerializeField] private Transform _player;
    
    private Vector3 _lastTipPos;
    private Quaternion _lastRotation;
    public Vector3 TipVelocity { get; private set; }
    public BaseElement Element { get; private set; }
    
    public Transform Tip => _tip;
    public LineRenderer LineRenderer => _lineRenderer;

    new void FixedUpdate()
    {
        base.FixedUpdate();

        TipVelocity = (Tip.position - _lastTipPos) / Time.fixedDeltaTime;
        _lastTipPos = Tip.position;
    }

    public void SetElement(BaseElement element) => Element = element;
    public BaseElement GetElement() => Element;
    public bool HasElement() => Element != null;

    public void SpawnElement(float speed, List<Vector3> points)
    {
        Vector3 start = points[0];
        Vector3 end = points[^1];

        Vector3 farthestPoint = start;
        float maxDistanceSqr = 0f;
        
        Vector3 line = end - start;
        float lineLengthSqr = line.sqrMagnitude;

        foreach (Vector3 p in points)
        {
            // Project point onto the line
            float t = Vector3.Dot(p - start, line) / lineLengthSqr;
            t = Mathf.Clamp01(t);

            Vector3 projection = start + line * t;
            float distanceSqr = (p - projection).sqrMagnitude;

            if (distanceSqr > maxDistanceSqr)
            {
                maxDistanceSqr = distanceSqr;
                farthestPoint = p;
            }
        }
        
        Vector3 direction = (farthestPoint - _player.position).normalized;
        direction.y = 0f;
        
        Debug.DrawLine(start, end, Color.red, 20f);
        Debug.DrawLine(farthestPoint, _player.position, Color.blue, 20f);
        Debug.DrawRay(farthestPoint, direction, Color.green, 20f);

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        GameObject slash = Instantiate(_elementPrefab, farthestPoint, rotation);
        slash.GetComponent<ElementalProjectile>()
            .Initialize(Element, speed, points);
    }
}