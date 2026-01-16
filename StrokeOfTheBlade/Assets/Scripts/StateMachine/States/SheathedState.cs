using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SheathedState : IState
{
    private KatanaManager _kM;
    private Elementmanager _eM;
    private SheathingState _sheathingState;

    private InputAction _activateAction;
    
    private ElementType _currentElement;
    private float _minRotationDif = 40;
    private float _vibrateAmplitude = 0.25f;

    public SheathedState(KatanaManager kM, Elementmanager eM, SheathingState sheathingState)
    {
        _kM = kM;
        _eM = eM;
        _sheathingState = sheathingState;
        _activateAction = _kM.Katana.GetAction("activate");
    }

    public void OnEnterState()
    {
        _currentElement = ElementType.Null; 
        _kM.Katana.Vibrate(_vibrateAmplitude, 0.25f);
        _kM.Sheath.Vibrate(_vibrateAmplitude, 0.25f);
    }

    public void OnExitState()
    {
        if(_currentElement != ElementType.Null)
            _kM.Katana.SetElement(_eM.GetElement(_currentElement));
        
        _kM.Katana.Vibrate(_vibrateAmplitude, 0.5f);
    }

    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        _sheathingState.OnFixedUpdate();
        
        float diffrence =  GetRotationZDiffrence();
        
        // rotate right
        if (diffrence > _minRotationDif)
        {
            ChangeCurrentElement(_activateAction.IsPressed() ? ElementType.Fire : ElementType.Water); 
        }
        // rototate left
        else if (diffrence < -_minRotationDif)
        {
            ChangeCurrentElement(_activateAction.IsPressed() ? ElementType.Wood : ElementType.Earth);
        }
    }

    private void ChangeCurrentElement(ElementType elementType)
    {
        if(_currentElement == elementType)
            return;
        
        _currentElement = elementType;
        AudioClip clip = _eM.GetElement(_currentElement).Data.OnChange;
        SoundManager.Instance.PlaySfx(clip);
        _kM.Sheath.Vibrate(_vibrateAmplitude, 0.25f);
    }
    private float GetRotationZDiffrence()
    {
        float startZ = _kM.Katana.transform.eulerAngles.z;
        float currentZ = _kM.Katana.HandRotation.eulerAngles.z;

        return Mathf.DeltaAngle(startZ, currentZ);
    }
}