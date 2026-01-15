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
        
        //Debug.Log("acceleration time: " + _accelerationTime);

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

        Vector3 first = transform.InverseTransformPoint(points[0]);
        Vector3 middle = transform.InverseTransformPoint(points[points.Count/2]);
        Vector3 last  = transform.InverseTransformPoint(points[^1]);
        

        _lineRenderer.positionCount = 3;
        _lineRenderer.SetPosition(0, first);
        _lineRenderer.SetPosition(1, middle);
        _lineRenderer.SetPosition(2, last);
    }
}
