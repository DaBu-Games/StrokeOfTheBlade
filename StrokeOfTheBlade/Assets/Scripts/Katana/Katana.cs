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
    [SerializeField] private ColliderDetection _colliderDetection;
    
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
        Vector3 middlePoint = GetPhysicalMiddle(points);
        
        Vector3 direction = (middlePoint - _player.position).normalized;
        direction.y = 0f;
        
        //Debug.DrawLine(start, end, Color.red, 20f);
        //Debug.DrawLine(middlePoint, _player.position, Color.blue, 20f);
        //Debug.DrawRay(middlePoint, direction, Color.green, 20f);
        
        List<Vector3> newPoints = new List<Vector3>();
        newPoints.Add(start);
        newPoints.Add(middlePoint);
        newPoints.Add(end);

        Quaternion rotation = Quaternion.LookRotation(direction);

        Vector3 target = _colliderDetection.GetClosestCollider(middlePoint, direction);
        
        GameObject slash = Instantiate(_elementPrefab, middlePoint, rotation);
        slash.GetComponent<ElementalProjectile>().Initialize(Element, speed, newPoints, target);
    }
    
    private Vector3 GetPhysicalMiddle(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
            return Vector3.zero;
        
        float totalLength = 0f;
        for (int i = 1; i < points.Count; i++)
            totalLength += Vector3.Distance(points[i - 1], points[i]);

        float halfLength = totalLength / 2f;
        
        float accumulated = 0f;
        for (int i = 1; i < points.Count; i++)
        {
            float segment = Vector3.Distance(points[i - 1], points[i]);
            if (accumulated + segment >= halfLength)
            {
                float t = (halfLength - accumulated) / segment;
                return Vector3.Lerp(points[i - 1], points[i], t);
            }
            accumulated += segment;
        }
        
        return points[points.Count - 1];
    }

}