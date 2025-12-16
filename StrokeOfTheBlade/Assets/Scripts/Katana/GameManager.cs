using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [SerializeField] private KatanaManager kM;
    
    private StateMachine _sm;
    
    //states
    private IdleState _idle;
    private SheathingState _sheathing;
    private SheathedState _sheathed;
    
    void Start()
    {
        _sm = new StateMachine();
        
        _idle = new IdleState(kM);
        _sheathing = new SheathingState(kM);
        _sheathed = new SheathedState(kM);
        

        _sm.AddTransition(new Transition(
            _idle,
            _sheathing,
            () => _sheathing.IsCloseToSheath() && kM.IsTipInMouth()
        ));
        
        _sm.AddTransition(new Transition(
            _sheathing,
            _idle,
            () => !_sheathing.IsCloseToSheath()
        ));
        
        _sm.AddTransition(new Transition(
            _sheathed,
            _sheathed,
            () => kM.IsTipInEnd()
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
