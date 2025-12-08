public class IdleState : IState
{
    private katanaManager _katanaManager;
    
    private float virbrateAmplitude = 0.5f;
    private float virbrateDuration = 0.5f;

    public IdleState(katanaManager katanaManager)
    {
        _katanaManager = katanaManager;
    }

    public void OnEnterState()
    {
        _katanaManager.VibrateSheath(virbrateAmplitude, virbrateDuration);
    }
    public void OnExitState() { }
    public void OnUpdate(){}
    public void OnFixedUpdate() { }
}