using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElementalProjectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer _lineRenderer;
    
    private BaseElement _element;
    
    private Transform _target;
    
    private float _accelerationTime = 0f;
    private float _currentSpeed = 0f;
    
    private float _elapsedSinceSpawn = 0f;
    private bool _startedMoving = false;

    public void Initialize(BaseElement element, float speed, List<Vector3> points, Transform target = null)
    {
        _element = element;
        _lineRenderer.material = _element.Data.Material;
        _accelerationTime = GetDurationWithSpeed(speed);

        if (target != null)
        {
            _target = target;
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

        if (_target != null)
        {
            Vector3 targetPos = _target.position;
            
            float distanceToTarget = Vector3.Distance(transform.position, targetPos);
            Vector3 control = transform.position + transform.forward * (distanceToTarget * 0.5f);
            
            float t = Mathf.Clamp01((_currentSpeed * Time.deltaTime) / distanceToTarget);
            
            Vector3 nextPos = Evaluate(transform.position, control, targetPos, t);
            
            Vector3 direction = (nextPos - transform.position).normalized;
            if (direction.sqrMagnitude > 0f)
                transform.rotation = Quaternion.LookRotation(direction);
            
            transform.position = nextPos;
        }
        else
        {
            transform.position += transform.forward * (_currentSpeed * Time.deltaTime);
        }
    }
    
    private Vector3 Evaluate(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        Vector3 ac = Vector3.Lerp(start, control, t);
        Vector3 cb = Vector3.Lerp(control, end, t);
        return Vector3.Lerp(ac, cb, t);
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
