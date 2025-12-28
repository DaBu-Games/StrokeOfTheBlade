using System;
using System.Collections.Generic;
using UnityEngine;

public class ElementalProjectile : MonoBehaviour
{
    [Header("Wave Shape")]
    [SerializeField] private float _radius = 10f;
    [SerializeField] private int _radialSegments = 8;
    [SerializeField] private float _arcDegrees = 140f;       // How wide the slash is
    [SerializeField] private float _forwardStretch = 1.4f;   // Length forward
    [SerializeField] private float _flatness = 0.4f;          // How flat the wave is
    [SerializeField] private float _taperPower = 1f;          // End sharpness

    [Header("Energy Effects")]
    [SerializeField] private float _pulseStrength = 0.15f;
    [SerializeField] private float _pulseFrequency = 0.5f;
    [SerializeField] private float _twistStrength = 0.1f;
    
    [Header("Movement")]
    [SerializeField] private float _startDelay = 200f;
    [SerializeField] private float _targetSpeed = 40f;
    [SerializeField] private float _minSlashSpeed = 2f;
    [SerializeField] private float _maxSlashSpeed = 40f;
    [SerializeField] private float _minDuraction = 0.2f;
    [SerializeField] private float _maxDuraction = 2f;
    
    [Header("References")]
    [SerializeField] private MeshFilter _meshFilter;
    
    private Mesh _mesh;
    private IElement _element;
    private float _accelerationTime = 0f;
    private float _currentSpeed = 0f;
    private float _elapsedSinceSpawn = 0f;
    private bool _startedMoving = false;

    public void Initialize(IElement element, float speed, List<Vector3> points)
    {
        _element = element;
        _accelerationTime = GetDurationWithSpeed(speed);
        
        Debug.Log("acceleration time: " + _accelerationTime);
        
        List<Vector3> localPoints = new List<Vector3>(points.Count);
        foreach (Vector3 p in points)
        {
            localPoints.Add(transform.InverseTransformPoint(p));
        }
        BuildMesh(localPoints);
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
        return Mathf.Lerp(_maxDuraction, _minDuraction, t);
    }

    private void BuildMesh(List<Vector3> points)
    {
        _mesh = new Mesh();
        _mesh.name = "SlashWaveMesh";

        float baseRadius = _radius;
        int ringCount = points.Count;
        int vertsPerRing = Mathf.Max(2, _radialSegments);

        Vector3[] vertices = new Vector3[ringCount * vertsPerRing];
        int[] triangles = new int[(ringCount - 1) * (vertsPerRing - 1) * 6];

        float arcRadians = _arcDegrees * Mathf.Deg2Rad;

        int v = 0;
        int t = 0;

        for (int i = 0; i < ringCount; i++)
        {
            Vector3 forward =
                i < ringCount - 1
                    ? (points[i + 1] - points[i]).normalized
                    : (points[i] - points[i - 1]).normalized;

            // Stable orientation
            Vector3 right = Vector3.Cross(forward, Vector3.up);
            if (right.sqrMagnitude < 0.001f)
                right = Vector3.Cross(forward, Vector3.right);

            right.Normalize();
            Vector3 up = Vector3.Cross(right, forward);

            float t01 = (float)i / (ringCount - 1);

            // Taper curve
            float taper = Mathf.Pow(Mathf.Sin(t01 * Mathf.PI), _taperPower);

            // Pulse
            float pulse =
                1f + Mathf.Sin(i * _pulseFrequency) * _pulseStrength;

            float radius = baseRadius * taper * pulse;

            float twist = i * _twistStrength;

            for (int j = 0; j < vertsPerRing; j++)
            {
                float arcT = j / (float)(vertsPerRing - 1);
                float angle =
                    -arcRadians * 0.5f +
                    arcT * arcRadians +
                    twist;

                float forwardPush =
                    Mathf.Max(0f, Mathf.Cos(angle)) *
                    radius * _forwardStretch;

                float side =
                    Mathf.Sin(angle) *
                    radius * _flatness;

                Vector3 offset =
                    right * side +
                    forward * forwardPush;

                vertices[v++] = points[i] + offset;
            }
        }

        // Build triangles
        for (int i = 0; i < ringCount - 1; i++)
        {
            for (int j = 0; j < vertsPerRing - 1; j++)
            {
                int a = i * vertsPerRing + j;
                int b = a + vertsPerRing;
                int c = a + 1;
                int d = b + 1;

                triangles[t++] = a;
                triangles[t++] = b;
                triangles[t++] = c;

                triangles[t++] = c;
                triangles[t++] = b;
                triangles[t++] = d;
            }
        }

        _mesh.vertices = vertices;
        _mesh.triangles = triangles;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        _meshFilter.sharedMesh = _mesh;
    }
}
