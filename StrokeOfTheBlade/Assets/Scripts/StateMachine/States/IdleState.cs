public class IdleState : IState
{
    private KatanaManager _kM;

    public IdleState(KatanaManager kM)
    {
        _kM = kM;
    }

    public void OnEnterState()
    {
        
    }

    public void OnExitState() { }
    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
    }
}