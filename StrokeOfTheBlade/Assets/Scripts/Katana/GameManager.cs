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
    private ChargedState _charged;
    
    void Start()
    {
        _sm = new StateMachine();
        
        _idle = new IdleState(kM);
        _sheathing = new SheathingState(kM);
        _sheathed = new SheathedState(kM, _sheathing, _inputAsset);
        _charged = new ChargedState(kM);

        // idle transition
        _sm.AddTransition(new Transition(
            _idle,
            _sheathing,
            () => _sheathing.IsCloseToSheath() && kM.IsTipInMouth
        ));
        
        // sheathing transitons
        _sm.AddTransition(new Transition(
            _sheathing,
            _charged,
            () => !_sheathing.IsCloseToSheath() && kM.Katana.IsCharged
        ));
        
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
        
        // charged transitions
        _sm.AddTransition(new Transition(
            _charged,
            _sheathing,
            () => _sheathing.IsCloseToSheath() && kM.IsTipInMouth
        ));
        
        _sm.AddTransition(new Transition(
            _charged,
            _idle,
            () => !kM.Katana.IsCharged
        ));
        
        _sm.SwitchState(_charged);
    }
    
    void Update()
    {
        _sm.OnUpdate();
    }

    void FixedUpdate()
    {
        _sm.OnFixedUpdate();
    }
    
    /*
    private void OnDrawGizmos()
    {
        if (_charged == null)
            return;

        var path = _charged.PathPositions;
        if (path == null || path.Count < 2)
            return;

        Gizmos.color = _charged.IsSlashingNow ? Color.cyan : Color.blue;

        float duration = _charged.IsSlashingNow ? 5f : 0.04f;

        for (int i = 1; i < path.Count; i++)
            Gizmos.DrawLine(path[i - 1], path[i]);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(path[0], duration);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(path[path.Count / 2], duration);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(path[^1], duration);
    }*/
}
