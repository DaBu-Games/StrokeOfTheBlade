using System.Collections.Generic;
using UnityEngine;

public class SheathingState : IState
{
    private KatanaManager _kM;
    private Rigidbody _kRb;
    private Rigidbody _sRb;
    private Animator _animator;
    
    [Range(0f, 1f)]
    private float _sheathAmount = 0f;
    private float _startDistance = 0f;
    

    [Header("Tuning")]
    private float _slideSpeedThreshold = 0.15f;
    private float _vibrateAmplitude = 0.25f;
    private float _vibrateDuration = 0.05f;

    public SheathingState(KatanaManager kM)
    {
        _kM = kM;
        _kRb = _kM.Katana.Rb;
        _sRb = _kM.Sheath.Rb;
        _animator = _kM.Animator;
    }

    public void OnEnterState()
    {
        _kM.Katana.FollowController(false);
        _animator.enabled = true;
        _animator.SetBool("IsSheathing", true);
        _startDistance =  Vector3.Distance(_kM.Katana.transform.position, _kM.Sheath.transform.position);
    }

    public void OnExitState()
    {
        if (!_kM.IsTipNearMouth())
        {
            _kM.Katana.Vibrate(1f, _vibrateDuration);
            _kM.Sheath.Vibrate(1f, _vibrateDuration);
            Debug.Log("force exit");
        }
        
        _animator.SetBool("IsSheathing", false);
        _animator.enabled = false;
        _kM.Katana.FollowController(true);
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        ApllyBladeVibration();
    }

    private void UpdateSheathAnimation()
    {
        float distance = Vector3.Distance(_kM.Katana.HandPosition, _kM.Sheath.HandPosition);
        _sheathAmount = Mathf.Clamp01(1f - (distance / _startDistance));
        
        _animator.SetFloat("SheathAmount", _sheathAmount);
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