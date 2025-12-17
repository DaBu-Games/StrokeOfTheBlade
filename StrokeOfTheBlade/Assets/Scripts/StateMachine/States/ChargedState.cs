using System.Collections.Generic;
using UnityEngine;

public class ChargedState : IState
{
    private KatanaManager _kM;

    private int _minPos = 5;
    private int _maxPos = 17;
    private float _minArcAngle = 45f;
    private float _minAngularSpeed = 360f;
    private float _minDistance = 1.7f;
    
    private List<Vector3> _pathPositions = new List<Vector3>();
    private Quaternion _lastRotation;
    
    /*
    public List<Vector3> PathPositions => _pathPositions;
    public bool IsSlashingNow => _isSlashing;*/

    public ChargedState(KatanaManager kM)
    {
        _kM = kM;
    }

    public void OnEnterState()
    {
        _lastRotation = _kM.Katana.HandRotation;
    }

    public void OnExitState()
    {
        _pathPositions.Clear();
    }
    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        SetPath();
        if (IsSlashing())
        {
            
        }
    }

    private void SetPath()
    {
        Vector3 tipPos = _kM.Katana.Tip.position;
        _pathPositions.Add(tipPos);

        if (_pathPositions.Count > _maxPos)
            _pathPositions.RemoveAt(0);
    }

    private bool IsSlashing()
    {
        if(_pathPositions.Count < _minPos)
            return false;
        
        float angularSpeed = GetAngularSpeed();
        if (angularSpeed < _minAngularSpeed)
        {
            //Debug.Log("agular speed: " + angularSpeed);
            return false;
        }
            

        float arcAngle = ComputeArcAngle();
        if (arcAngle < _minArcAngle)
        {
            //Debug.Log("arc angle: " + arcAngle);
            return false;
        }
            

        float distance = ComputeDistance();
        if (distance < _minDistance)
        {
            //Debug.Log("Distance: " + distance);
            return false;
        }

        if (!IsEdgeLeading())
        {
            //Debug.Log("not face forward");
            return false;
        }

        return true;
    }

    private float GetAngularSpeed()
    {
        Quaternion currentRot = _kM.Katana.HandRotation;
        Quaternion difference = currentRot * Quaternion.Inverse(_lastRotation);
        
        difference.ToAngleAxis(out float angle, out _);
        
        if (angle > 180f)
            angle -= 360f;

        float angularSpeed = Mathf.Abs(angle) / Time.fixedDeltaTime;

        _lastRotation = currentRot;
        return angularSpeed;
    }
    
    private float ComputeArcAngle()
    {
        Vector3 start = _pathPositions[0];
        Vector3 mid   = _pathPositions[_pathPositions.Count / 2];
        Vector3 end   = _pathPositions[^1];

        Vector3 a = (start - mid).normalized;
        Vector3 b = (end - mid).normalized;

        return Vector3.Angle(a, b);
    }
    
    private float ComputeDistance()
    {
        Vector3 start = _pathPositions[0];
        Vector3 end   = _pathPositions[^1];

        return Vector3.Distance(start, end);
    }
    
    private bool IsEdgeLeading()
    {
        Vector3 prev = _pathPositions[^2];
        Vector3 current = _pathPositions[^1];
        Vector3 velocity = current - prev;
        float dot = Vector3.Dot(velocity.normalized, -_kM.Katana.transform.up);
        
        return dot > 0.5f;
    }
}