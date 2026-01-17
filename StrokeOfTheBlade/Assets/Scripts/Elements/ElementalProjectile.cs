using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementalProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer _lineRenderer;
    
    private BaseElement _element;
    private readonly int _steps = 20;

    private Vector3 _start;
    private Vector3 _control;
    private Vector3 _target;
    
    private float _accelerationTime = 0f;
    private float _currentSpeed = 0f;
    
    private float _elapsedSinceSpawn = 0f;
    private bool _startedMoving = false;
    
    private float _travelledDistance = 0f;
    private float _curveLength = 0f;

    public void Initialize(BaseElement element, float speed, List<Vector3> points, Vector3 target = default)
    {
        _element = element;
        _lineRenderer.material = _element.Data.Material;
        _accelerationTime = GetDurationWithSpeed(speed);

        if (target != Vector3.zero)
        {
            _start = transform.position;
            _target = target;
            float distanceToTarget = Vector3.Distance(transform.position, _target);
            _control = _start + (transform.forward * (distanceToTarget * 0.5f));
            _curveLength = ApproximateCurveLength();
        }
        
        SetPoints(points);
    }

    void Update()
    {
        if (!_startedMoving)
        {
            _elapsedSinceSpawn += Time.deltaTime;
            
            if (_elapsedSinceSpawn >= _element.Data.StartDelay)
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

        if (_target != Vector3.zero)
        {
            _travelledDistance += _currentSpeed * Time.deltaTime;
            float t = Mathf.Clamp01(_travelledDistance / _curveLength);

            Vector3 currentPos = Evaluate(t);
            Vector3 nextPos = Evaluate(Mathf.Min(t + 0.01f, 1f));

            Vector3 direction = (nextPos - currentPos).normalized;

            if (direction.sqrMagnitude > 0f)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            transform.position = currentPos;

            if (t >= 1f)
            {
                _target = Vector3.zero;
            }
        }
        else
        {
            transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
        }
    }
    
    private Vector3 Evaluate(float t)
    {
        Vector3 ac = Vector3.Lerp(_start, _control, t);
        Vector3 cb = Vector3.Lerp(_control, _target, t);
        return Vector3.Lerp(ac, cb, t);
    }
    
    private float ApproximateCurveLength()
    {
        float length = 0f;
        Vector3 prev = Evaluate(0f);

        for (int i = 1; i <= _steps; i++)
        {
            float t = i / (float)_steps;
            Vector3 p = Evaluate(t);
            length += Vector3.Distance(prev, p);
            prev = p;
        }

        return length;
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
