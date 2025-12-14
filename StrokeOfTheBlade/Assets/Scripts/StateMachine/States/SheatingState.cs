using System.Collections.Generic;
using UnityEngine;

public class SheathingState : IState
{
    private KatanaManager _kM;
    private Rigidbody _kRb;
    private Rigidbody _sRb;
    
    private List<Transform> _points;

    private float _t;
    private float _pathLength;

    private Vector3 _tipLocalOffset;
    private Vector3 _lastHandPos;

    [Header("Tuning")]
    private float _movementSensitivity = 1.0f;
    private float _slideSpeedThreshold = 0.15f;
    private float _vibrateAmplitude = 0.25f;
    private float _vibrateDuration = 0.05f;

    public SheathingState(KatanaManager kM)
    {
        _kM = kM;
        _kRb = _kM.Katana.KRb;
        _sRb = _kM.Sheath.SRb;
        _points = _kM.Sheath.Points;
        
        CalculatePathLength();
    }

    public void OnEnterState()
    {
        _kRb.isKinematic = true;
        _kRb.detectCollisions = false;
        
        _kM.Katana.FollowController(false);
        
        _t = EstimateInitialT();
        _lastHandPos = _kM.Katana.HandPosition;
        _tipLocalOffset = _kRb.transform.InverseTransformPoint(_kM.Katana.Tip.position);
    }

    public void OnExitState()
    {
        if (!_kM.IsTipNearMouth())
        {
            _kM.Katana.Vibrate(1f, _vibrateDuration);
            _kM.Sheath.Vibrate(1f, _vibrateDuration);
            Debug.Log("force exit");
        }
        
        _kM.Katana.FollowController(true);
        
        _kRb.isKinematic = false;
        _kRb.detectCollisions = true;
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        UpdateSheathMotion();
        ApllyBladeVibration();
    }
    
    private void UpdateSheathMotion()
    {
        Vector3 handPos = _kM.Katana.HandPosition;
        Vector3 handDelta = handPos - _lastHandPos;
        
        Vector3 pathDir = GetPathDirection(_t);
        float push = Vector3.Dot(handDelta, pathDir);
        _t += (push * _movementSensitivity) / _pathLength;
        _t = Mathf.Clamp01(_t);

        Vector3 forward = GetPathDirection(_t);
        if (forward.sqrMagnitude > 0.0001f)
        {
            Vector3 up = _kM.Sheath.transform.up;
            _kRb.MoveRotation(Quaternion.LookRotation(forward, up));
        }
        
        Vector3 worldTipOffset = _kRb.rotation * _tipLocalOffset;
        Vector3 targetTipPos = GetPositionOnPath(_t);
        //float initialOffset = 0.65f; // adjust to taste
        //_kRb.MovePosition(targetTipPos - _kRb.transform.forward * initialOffset);
        _kRb.MovePosition(targetTipPos);
        
        _lastHandPos = handPos;
    }
    
    private Vector3 GetPositionOnPath(float t)
    {
        if (_points == null || _points.Count == 0)
            return Vector3.zero;

        t = Mathf.Clamp01(t);

        float scaled = t * (_points.Count - 1);
        int i = Mathf.FloorToInt(scaled);
        int j = i + 1;

        if (j >= _points.Count)
        {
            j = i;      // clamp j to last index
            scaled = i; // localT will be 0
        }

        float localT = scaled - i;

        return Vector3.Lerp(
            _points[i].position,
            _points[j].position,
            localT
        );
    }

    private Vector3 GetPathDirection(float t)
    {
        float t2 = Mathf.Clamp01(t + 0.01f);
        return (GetPositionOnPath(t2) - GetPositionOnPath(t)).normalized;
    }
    
    private float EstimateInitialT()
    {
        Vector3 tipPos = _kM.Katana.Tip.position;

        float bestT = 0f;
        float bestDist = float.MaxValue;

        for (int i = 0; i < _points.Count; i++)
        {
            float dist = Vector3.Distance(tipPos, _points[i].position);
            if (dist < bestDist)
            {
                bestDist = dist;
                bestT = i / (float)(_points.Count - 1);
            }
        }

        return bestT;
    }
    
    private void CalculatePathLength()
    {
        _pathLength = 0f;
        for (int i = 0; i < _points.Count - 1; i++)
        {
            _pathLength += Vector3.Distance(
                _points[i].position,
                _points[i + 1].position
            );
        }
    }

    private void ApllyBladeVibration()
    {
        float speed = _kRb.linearVelocity.magnitude + _sRb.linearVelocity.magnitude;

        if (speed > _slideSpeedThreshold)
        {
            _kM.Katana.Vibrate(_vibrateAmplitude, 0.05f); 
        }
    }
}