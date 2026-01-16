using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

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

    private void OnTriggerEnter(Collider other)
    {
        _element.OnHit(other);
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
        
        _lineRenderer.positionCount = _element.Data.LineSegments;

        for (int i = 0; i < _element.Data.LineSegments; i++)
        {
            float t = i / (float)(_element.Data.LineSegments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            
            if (i != 0 && i != _element.Data.LineSegments - 1)
            {
                float curveFactor = Mathf.Sin(t * Mathf.PI);
                point += forwardOffset * curveFactor;
            }

            _lineRenderer.SetPosition(i, point);
        }
    }
}
