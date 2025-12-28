using System.Collections.Generic;
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
    public IElement Element { get; private set; }
    
    public Transform Tip => _tip;
    public LineRenderer LineRenderer => _lineRenderer;

    new void FixedUpdate()
    {
        base.FixedUpdate();

        TipVelocity = (Tip.position - _lastTipPos) / Time.fixedDeltaTime;
        _lastTipPos = Tip.position;
    }

    public void SetElement(IElement element) => Element = element;
    public bool HasElement() => Element != null;

    public void SpawnElement(float speed, List<Vector3> points)
    {
        GameObject slash = Instantiate(_elementPrefab, points[0], Quaternion.identity);
        slash.GetComponent<ElementalProjectile>().Initialize(Element, speed, points);
    }
}