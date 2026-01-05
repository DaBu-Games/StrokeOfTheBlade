using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Katana : BaseController
{
    [Header("katana value's")]
    [SerializeField] private Transform _tip;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _elementPrefab;
    
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
        int index = FindMaxDeviationIndex(points);
        
        Vector3 t1 = (points[index] - points[0]).normalized;
        Vector3 t2 = (points[^1] - points[index]).normalized;

        Vector3 direction = (t1 - t2).normalized;
        
        direction.y = 0f;
        direction.Normalize();

        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        
        Vector3 spawnPos = points[points.Count / 2];
        
        GameObject slash = Instantiate(_elementPrefab, spawnPos, rotation);
        slash.GetComponent<ElementalProjectile>().Initialize(Element, speed, points);

        Element = null;
    }
    
    private int FindMaxDeviationIndex(List<Vector3> points)
    {
        Vector3 start = points[0];
        Vector3 end = points[^1];

        Vector3 lineDir = (end - start).normalized;

        float maxDist = 0f;
        int bestIndex = points.Count / 2;

        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector3 toPoint = points[i] - start;
            
            Vector3 projected = Vector3.Project(toPoint, lineDir);
            
            float dist = (toPoint - projected).magnitude;

            if (dist > maxDist)
            {
                maxDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}