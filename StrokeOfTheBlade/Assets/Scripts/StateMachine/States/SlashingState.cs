using System.Collections.Generic;
using UnityEngine;

public class SlashingState : IState
{
    private KatanaManager _kM;
    private LineRenderer _lineRenderer;

    private float _minSlashSpeed = 2f;
    private float _minForwardDot = 0.6f;
    
    private List<Vector3> _pathPositions = new List<Vector3>();
    private Quaternion _startRotation;

    public SlashingState(KatanaManager kM)
    {
        _kM = kM;
        _lineRenderer = _kM.Katana.LineRenderer;
    }

    public void OnEnterState()
    {
        _lineRenderer.enabled = true;
        _startRotation = _kM.Katana.HandRotation;
    }

    public void OnExitState()
    {
        _pathPositions.Clear();
        _lineRenderer.enabled = false;
    }
    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        
    }

    public bool IsAboveSpeed()
    {
        return _kM.Katana.TipVelocity.magnitude >= _minSlashSpeed;
    }

    public bool IsMovingForward()
    {
        Vector3 velocityDir = _kM.Katana.TipVelocity.normalized;
        Vector3 bladeForward = -_kM.Katana.transform.up;

        float dot = Vector3.Dot(velocityDir, bladeForward);

        return dot > _minForwardDot;
    }
}