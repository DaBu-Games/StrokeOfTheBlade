using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private KatanaManager katanaManager;
    
    private StateMachine _sm;
    
    //states
    private IdleState _idle;
    private SheathingState _sheathing;
    
    void Start()
    {
        _sm = new StateMachine();
        
        _idle = new IdleState(katanaManager);
        _sheathing = new SheathingState(katanaManager);
        
        _sm.AddTransition(new Transition(
            _idle,
            _sheathing,
            () => katanaManager.IsSheating() && katanaManager.IsTipNearMouth()
        ));
        
        _sm.AddTransition(new Transition(
            _sheathing,
            _idle,
            () => !katanaManager.IsSheating()
        ));
        
        _sm.SwitchState(_idle);
    }
    
    void Update()
    {
        _sm.OnUpdate();
    }

    void FixedUpdate()
    {
        _sm.OnFixedUpdate();
    }
}
