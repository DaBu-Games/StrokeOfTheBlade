using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [SerializeField] private KatanaManager kM;
    [SerializeField] private InputActionAsset _inputAsset;
    
    private StateMachine _sm;
    
    //states
    private IdleState _idle;
    private SheathingState _sheathing;
    private SheathedState _sheathed;
    private SlashingState _slashing;
    
    void Start()
    {
        _sm = new StateMachine();
        
        _idle = new IdleState(kM);
        _sheathing = new SheathingState(kM);
        _sheathed = new SheathedState(kM, _sheathing, _inputAsset);
        _slashing = new SlashingState(kM);

        // idle transition
        _sm.AddTransition(new Transition(
            _idle,
            _sheathing,
            () => _sheathing.IsCloseToSheath() && kM.IsTipInMouth
        ));
        
        _sm.AddTransition(new Transition(
            _idle,
            _slashing,
            () => kM.Katana.IsCharged && _slashing.IsMovingForward() && _slashing.IsAboveSpeed()
        ));
        
        // sheathing transitons
        _sm.AddTransition(new Transition(
            _sheathing,
            _idle,
            () => !_sheathing.IsCloseToSheath()
        ));
        
        _sm.AddTransition(new Transition(
            _sheathing,
            _sheathed,
            () => kM.IsTipInEnd
        ));
        
        // sheathed transition
        _sm.AddTransition(new Transition(
            _sheathed,
            _sheathing,
            () => !kM.IsTipInEnd
        ));
        
        //slashing transition
        _sm.AddTransition(new Transition(
            _slashing,
            _idle,
            () => !kM.Katana.IsCharged || !_slashing.IsAboveSpeed()
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
