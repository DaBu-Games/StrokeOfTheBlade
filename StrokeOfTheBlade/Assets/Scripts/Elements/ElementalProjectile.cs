using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementalProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer _lineRenderer;
    
    private BaseElement _element;
    private float _accelerationTime = 0f;
    private float _currentSpeed = 0f;
    private float _elapsedSinceSpawn = 0f;
    private bool _startedMoving = false;

    public void Initialize(BaseElement element, float speed, List<Vector3> points)
    {
        _element = element;
        _lineRenderer.material = _element.Data.Material;
        _accelerationTime = GetDurationWithSpeed(speed);

        SetPoints(points);
    }

    void Update()
    {
        _elapsedSinceSpawn += Time.deltaTime;
        
        if (!_startedMoving)
        {
            if (_elapsedSinceSpawn >= _element.Data.StartDelay / 1000f)
            {
                _startedMoving = true;
                _elapsedSinceSpawn = 0f;
            }
            else
                return;
        }
        
        if (_currentSpeed < _element.Data.TargetSpeed)
        {
            _currentSpeed += (_element.Data.TargetSpeed / _accelerationTime) * Time.deltaTime;
            
            if (_currentSpeed > _element.Data.TargetSpeed)
            {
                _currentSpeed = _element.Data.TargetSpeed;
            }
        }

        transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
    }
    
    public BaseElement GetElement() => _element;

    private void OnTriggerEnter(Collider other)
    {
        _element.OnHit(other, this.gameObject);
    }

    private float GetDurationWithSpeed(float speed)
    {
        float t = Mathf.InverseLerp(_element.Data.MinProjectileSpeed, _element.Data.MaxProjectileSpeed, speed);
        return Mathf.Lerp(_element.Data.MaxDuration, _element.Data.MinDuration, t);
    }

    private void SetPoints(List<Vector3> points)
    {
        _lineRenderer.useWorldSpace = false;

        Vector3 start = transform.InverseTransformPoint(points[0]);
        Vector3 end  = transform.InverseTransformPoint(points[^1]);
        
        Vector3 forwardOffset = transform.InverseTransformDirection(transform.forward) * _element.Data.ForwardCurve;

        int segments = _element.Data.LineSegments;
        _lineRenderer.positionCount = segments;
        
        Vector3[] linePoints = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)(segments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            
            if (i != 0 && i != segments - 1)
            {
                float curveFactor = Mathf.Sin(t * Mathf.PI);
                point += forwardOffset * curveFactor;
            }
            
            linePoints[i] = point;
        }
        
        _lineRenderer.SetPositions(linePoints);
        CreateBoxCollider(linePoints);
    }
    
    private void CreateBoxCollider(Vector3[] points)
    {
        BoxCollider box = gameObject.AddComponent<BoxCollider>();
        box.isTrigger = true;
        
        Vector3 min = points[0];
        Vector3 max = points[0];

        foreach (var p in points)
        {
            min = Vector3.Min(min, p);
            max = Vector3.Max(max, p);
        }

        Vector3 size = max - min;
        
        float minThickness = _lineRenderer.startWidth;
        if (size.y < minThickness) size.y = minThickness;
        if (size.x < minThickness) size.x = minThickness; 
        if (size.z < minThickness) size.z = minThickness;

        box.center = (min + max) * 0.5f;
        box.size = size;
    }
}
