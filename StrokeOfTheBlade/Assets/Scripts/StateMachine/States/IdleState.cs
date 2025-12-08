public class IdleState : IState
{
    private KatanaManager _katanaManager;
    
    private float virbrateAmplitude = 0.5f;
    private float virbrateDuration = 0.5f;

    public IdleState(KatanaManager katanaManager)
    {
        _katanaManager = katanaManager;
    }

    public void OnEnterState()
    {
        _katanaManager.Sheath.Vibrate(virbrateAmplitude, virbrateDuration);
    }
    public void OnExitState() { }
    public void OnUpdate(){}
    public void OnFixedUpdate() { }
}