using UnityEngine;

public class SheathingState : IState
{
    private KatanaManager _katanaManager;
    private Rigidbody _katanaRb;
    
    private float virbrateAmplitude = 0.5f;
    private float virbrateDuration = 0.5f;

    public SheathingState(KatanaManager sensor)
    {
        this._katanaManager = sensor;
        this._katanaRb = _katanaManager.Katana.Rb;
    }

    public void OnEnterState()
    {
        _katanaManager.Katana.Vibrate(virbrateAmplitude, virbrateDuration);
    }

    public void OnExitState()
    {

    }

    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        
    }
}