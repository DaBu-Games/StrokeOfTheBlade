using System.Collections.Generic;
using UnityEngine;

public class SheathingState : IState
{
    private KatanaManager _kM;
    private Rigidbody _kRb;
    private Rigidbody _sRb;
    private Animation _animation;
    private AnimationClip _sheathClip;
    
    [Range(0f, 1f)]
    private float _sheathAmount = 0f;

    [Header("Tuning")]
    private float _minDistance = 0.15f;
    private float _maxDistance;
    private float _slideSpeedThreshold = 0.4f;
    private float _vibrateAmplitude = 0.15f;
    private float _vibrateDuration = 0.05f;

    public SheathingState(KatanaManager kM)
    {
        _kM = kM;
        _kRb = _kM.Katana.Rb;
        _sRb = _kM.Sheath.Rb;
        
        _animation = _kM.Sheath.Animation;
        _sheathClip = _animation.GetClip("sheathing");
        _maxDistance = Vector3.Distance(_kM.Katana.transform.position, _kM.Katana.Tip.position);
    }

    public void OnEnterState()
    {
        _kM.Katana.FollowController(false);
        _kM.Katana.transform.SetParent(_kM.Sheath.transform, true);
        
        _animation[_sheathClip.name].speed = 0f;
        _animation.Play(_sheathClip.name);
        
        SetSheathAmount();
    }

    public void OnExitState()
    {
        _animation.Stop();
        _kM.Katana.transform.SetParent(null, false);
        _kM.Katana.FollowController(true);
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        SetSheathAmount();
        ApllyBladeVibration();
    }

    private void SetSheathAmount()
    {
        float distance = Vector3.Distance(_kM.Katana.HandPosition, _kM.Sheath.HandPosition);
        
        distance = Mathf.Clamp(distance, _minDistance, _maxDistance);
        
        _sheathAmount = Mathf.InverseLerp(_maxDistance, _minDistance, distance);
        
        _animation[_sheathClip.name].time = _sheathClip.length * _sheathAmount;
        _animation.Sample(); 
    }

    private void ApllyBladeVibration()
    {
        float speed = _kRb.linearVelocity.magnitude + _sRb.linearVelocity.magnitude;

        if (speed > _slideSpeedThreshold)
        {
            _kM.Katana.Vibrate(_vibrateAmplitude, Time.fixedDeltaTime); 
        }
    }

    public bool IsCloseToSheath()
    {
        return Vector3.Distance(_kM.Katana.HandPosition, _kM.Sheath.HandPosition) < _maxDistance;
    }
}