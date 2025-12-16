using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SheathedState : IState
{
    private KatanaManager _kM;
    private SheathingState _sheathingState;
    
    private InputAction _leftTrigger;
    private InputAction _rightTrigger;

    public SheathedState(KatanaManager kM, SheathingState sheathingState, InputActionAsset inputAsset)
    {
        _kM = kM;
        _sheathingState = sheathingState;
        
        _leftTrigger = inputAsset.FindActionMap("XRI Left Interaction").FindAction("Select");
        _rightTrigger = inputAsset.FindActionMap("XRI Right Interaction").FindAction("Select");
    }

    public void OnEnterState()
    {
        _kM.Katana.SetCharged(true);
        /*
        _leftTrigger.performed += ChangeDevice;
        _rightTrigger.performed += ChangeDevice;
        */
    }

    public void OnExitState() { }

    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        _sheathingState.OnFixedUpdate();
    }
    
    /*
    private void ChangeDevice(InputAction.CallbackContext ctx)
    {
        Debug.Log("Change device called");
        
        XRNode katanaNode = _kM.Katana.ControllerNode;
        XRNode sheathNode = _kM.Sheath.ControllerNode;
        
        _kM.Katana.SetDevice(sheathNode);
        _kM.Sheath.SetDevice(katanaNode);
    }*/
}