using UnityEngine;

public class SheathingState : IState
{
    private KatanaManager _katanaManager;
    private Rigidbody _katanaRb;
    
    private float pullStrength = 20f; 
    private float virbrateAmplitude = 0.5f;
    private float virbrateDuration = 0.5f;

    public SheathingState(KatanaManager sensor)
    {
        _katanaManager = sensor;
        _katanaRb = _katanaManager.Katana.KRb;
    }

    public void OnEnterState()
    {
        _katanaManager.Katana.Vibrate(virbrateAmplitude, virbrateDuration);
        _katanaRb.isKinematic = false;
    }

    public void OnExitState()
    {
        if (_katanaManager.ForcedExitSheating())
        {
            _katanaManager.Katana.Vibrate(virbrateAmplitude, virbrateDuration);
        }
    }

    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        Vector3 sheathMouth = _katanaManager.Sheath.Mouth.position;
        Vector3 sheathEnd = _katanaManager.Sheath.End.position;
        Vector3 tipPos = _katanaManager.Katana.Tip.position;

        Vector3 sheathAxis = (sheathEnd - sheathMouth).normalized;

        // ------------------------
        // Option 1: Pull force
        // ------------------------
        Vector3 dirToMouth = (sheathMouth - tipPos);
        float distanceToMouth = dirToMouth.magnitude;
        Vector3 pullForce = dirToMouth.normalized * (pullStrength * Mathf.Clamp01(distanceToMouth * 10f));

        _katanaRb.AddForce(pullForce, ForceMode.Acceleration);
    }
}