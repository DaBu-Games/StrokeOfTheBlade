using UnityEngine;

public class SheathedState : IState
{
    private KatanaManager _katanaManager;

    public SheathedState(KatanaManager katanaManager)
    {
        _katanaManager = katanaManager;
    }
    
    public void OnEnterState()
    {
        _katanaManager.Katana.Vibrate(0.5f, 0.5f);
        _katanaManager.Sheath.Vibrate(0.5f, 0.5f);
    }

    public void OnExitState()
    {
        
    }

    public void OnUpdate()
    {
        
    }

    public void OnFixedUpdate()
    {
        
    }
}