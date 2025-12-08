public class IdleState : IState
{
    private KatanaManager _katanaManager;

    public IdleState(KatanaManager katanaManager)
    {
        _katanaManager = katanaManager;
    }

    public void OnEnterState()
    {
        
    }
    public void OnExitState() { }
    public void OnUpdate(){}
    public void OnFixedUpdate() { }
}