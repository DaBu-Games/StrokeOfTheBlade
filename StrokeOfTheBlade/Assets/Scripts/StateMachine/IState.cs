

public interface IState
{
    void OnEnterState();
    void OnExitState();
    void OnUpdate();
    void OnFixedUpdate();
}
