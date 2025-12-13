using System.Collections.Generic;
using UnityEngine;

public class SheathingState : IState
{
    private KatanaManager _kM;
    private Rigidbody _kRb;
    private Rigidbody _sRb;
    
    private List<Transform> _points;

    private float _smoothing = 0.8f;
    
    private float _slideSpeedThreshold = 0.15f;
    private float _virbrateAmplitude  = 0.25f;
    
    private float _virbrateDuration = 0.1f;

    public SheathingState(KatanaManager kM)
    {
        _kM = kM;
        _kRb = _kM.Katana.KRb;
        _sRb = _kM.Sheath.SRb;
        _points = _kM.Sheath.Points;
    }

    public void OnEnterState()
    {
    }

    public void OnExitState()
    {
        if (!_kM.IsTipNearMouth())
        {
            _kM.Katana.Vibrate(1f, _virbrateDuration);
            _kM.Sheath.Vibrate(1f, _virbrateDuration);
            Debug.Log("force exit");
        }
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        GuideBlade();
        ApllyBladeVibration();
    }
    
    private void GuideBlade()
    {
        
        Vector3 tipPos = _kM.Katana.Tip.position;
        Vector3 targetOffset = Vector3.zero;
        float closestDistance = float.MaxValue;

        // Find closest point
        foreach (var point in _points)
        {
            Vector3 worldPoint = point.position;
            Vector3 offset = worldPoint - tipPos;
            float dist = offset.magnitude;

            if (dist < closestDistance)
            {
                closestDistance = dist;
                targetOffset = offset;
            }
        }

        // Lock the axis you don't want pulled (e.g., z)
        targetOffset.z = 0f;

        // Scale pull by distance so it’s stronger when farther
        float pullStrength = Mathf.Clamp01(closestDistance * 10f); // tweak multiplier
        Vector3 scaledOffset = targetOffset * pullStrength;

        // Smooth it so it doesn’t teleport
        Vector3 smoothedOffset = Vector3.Lerp(Vector3.zero, scaledOffset, _smoothing);

        _kM.Katana.AddPositionOffset(smoothedOffset);
    }

    private void ApllyBladeVibration()
    {
        float speed = _kRb.linearVelocity.magnitude + _sRb.linearVelocity.magnitude;

        if (speed > _slideSpeedThreshold)
        {
            _kM.Katana.Vibrate(_virbrateAmplitude, 0.05f); 
        }
    }
}