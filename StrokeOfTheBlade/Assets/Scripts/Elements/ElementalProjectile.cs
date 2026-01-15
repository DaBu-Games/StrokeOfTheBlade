using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementalProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _startDelay = 100f;
    [SerializeField] private float _targetSpeed = 40f;
    [SerializeField] private float _minSlashSpeed = 2f;
    [SerializeField] private float _maxSlashSpeed = 10f;
    [SerializeField] private float _minDuration = 0.2f;
    [SerializeField] private float _maxDuration = 2f;
    [SerializeField] private float _forwardCurve = 1f;
    [SerializeField] private int _segments = 5;
    
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
            if (_elapsedSinceSpawn >= _startDelay / 1000f)
            {
                _startedMoving = true;
                _elapsedSinceSpawn = 0f;
            }
            else
                return;
        }
        
        if (_currentSpeed < _targetSpeed)
        {
            _currentSpeed += (_targetSpeed / _accelerationTime) * Time.deltaTime;
            
            if (_currentSpeed > _targetSpeed)
            {
                _currentSpeed = _targetSpeed;
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
        float t = Mathf.InverseLerp(_minSlashSpeed, _maxSlashSpeed, speed);
        return Mathf.Lerp(_maxDuration, _minDuration, t);
    }

    private void SetPoints(List<Vector3> points)
    {
        _lineRenderer.useWorldSpace = false;

        Vector3 start = transform.InverseTransformPoint(points[0]);
        Vector3 end  = transform.InverseTransformPoint(points[^1]);
        
        Vector3 forwardOffset = transform.InverseTransformDirection(transform.forward) * _forwardCurve;
        
        _lineRenderer.positionCount = _segments;

        for (int i = 0; i < _segments; i++)
        {
            float t = i / (float)(_segments - 1);
            Vector3 point = Vector3.Lerp(start, end, t);
            
            if (i != 0 && i != _segments - 1)
            {
                float curveFactor = Mathf.Sin(t * Mathf.PI);
                point += forwardOffset * curveFactor;
            }

            _lineRenderer.SetPosition(i, point);
        }
    }
}
