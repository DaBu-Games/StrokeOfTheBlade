using UnityEngine;

public class SheathingState : IState
{
    private katanaManager _katanaManager;
    private Rigidbody _katanaRb;
    private float _pullStrength = 20f;
    
    private float virbrateAmplitude = 0.5f;
    private float virbrateDuration = 0.5f;

    public SheathingState(katanaManager sensor)
    {
        this._katanaManager = sensor;
        this._katanaRb = _katanaManager.KatanaRb;
    }

    public void OnEnterState()
    {
        _katanaManager.VibrateKatana(virbrateAmplitude, virbrateDuration);
    }

    public void OnExitState()
    {

    }

    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        if (_katanaManager.IsTipTooFar())
            return;
        
        Vector3 dir = (_katanaManager.SheathMouth.position - _katanaManager.KatanaTip.position).normalized;
        float dist = _katanaManager.DistanceToMouth();
        
        float force = _pullStrength * Mathf.Clamp01(dist * 10f);

        _katanaRb.AddForce(dir * force, ForceMode.Acceleration);
    }
}