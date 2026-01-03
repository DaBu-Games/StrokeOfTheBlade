using System.Collections.Generic;
using UnityEngine;

public class SlashingState : IState
{
    private KatanaManager _kM;
    private LineRenderer _lineRenderer;

    // is slashing variables
    private float _minSlashSpeed = 2f;
    private float _minForwardDot = 0.6f;
    private float _minPointDistance = 0.01f;
    
    //slash check variables
    private int _minPathCount = 5;
    private float _minSlashLength = 1f;
    
    private List<Vector3> _pathPositions = new List<Vector3>();
    private float _startSlashTime;

    public SlashingState(KatanaManager kM)
    {
        _kM = kM;
        _lineRenderer = _kM.Katana.LineRenderer;
    }

    public void OnEnterState()
    {
        _lineRenderer.enabled = true;
        _startSlashTime = Time.time;
        _lineRenderer.material = _kM.Katana.GetElement().Data.Material;
    }

    public void OnExitState()
    {
        if (IsCleanSlash())
        {
            _kM.Katana.SpawnElement(GetAverageSlashSpeed(), _pathPositions);
        }
        
        _pathPositions.Clear();
        _startSlashTime = Mathf.Epsilon;
        _lineRenderer.positionCount = 0;
        _lineRenderer.enabled = false;
    }
    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        Vector3 tipPos = _kM.Katana.Tip.position;

        if (_pathPositions.Count == 0 || Vector3.Distance(_pathPositions[^1], tipPos) > _minPointDistance)
        {
            _pathPositions.Add(tipPos);
        
            _lineRenderer.positionCount = _pathPositions.Count;
            _lineRenderer.SetPosition(_pathPositions.Count - 1, tipPos);
        }
    }
    
    private bool IsCleanSlash()
    {
        if (_pathPositions.Count < _minPathCount) 
            return false;

        if (GetSlashLength() < _minSlashLength)
            return false;
        
        return true;
    }

    private float GetAverageSlashSpeed()
    {
        if (_pathPositions.Count < _minPathCount)
            return 0f;
        
        float time = Time.time - _startSlashTime;
        
        if(time <= Mathf.Epsilon)
            return 0f;
        
        return GetSlashLength() / time;
    }

    private float GetSlashLength()
    {
        float length = 0f;

        for (int i = 1; i < _pathPositions.Count; i++)
        {
            length += Vector3.Distance(
                _pathPositions[i],
                _pathPositions[i - 1]
            );
        }

        return length;
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