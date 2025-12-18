using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Katana : BaseController
{
    [Header("katana value's")]
    [SerializeField] private Transform _tip;
    [SerializeField] private bool _isCharged;
    [SerializeField] private LineRenderer _lineRenderer;
    
    private Vector3 _lastTipPos;
    public Vector3 TipVelocity { get; private set; }
    
    public Transform Tip => _tip;
    public void SetCharged(bool isCharged) => _isCharged = isCharged;
    public bool IsCharged => _isCharged;
    public LineRenderer LineRenderer => _lineRenderer;

    void FixedUpdate()
    {
        base.FixedUpdate();
        
        TipVelocity = (Tip.position - _lastTipPos) / Time.fixedDeltaTime;
        _lastTipPos = Tip.position;
    }
}