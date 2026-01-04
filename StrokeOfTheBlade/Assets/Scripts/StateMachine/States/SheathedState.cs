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
    private Quaternion _orginalRotation;
    
    private ElementType _currentElement;
    private float _minRotationDif = 40;
    private float _vibrateAmplitude = 0.5f;

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
        _orginalRotation = _kM.Katana.HandRotation;
    }

    public void OnExitState()
    {
        if(_currentElement != ElementType.Null)
            _kM.Katana.SetElement(_eM.GetElement(_currentElement));
    }

    public void OnUpdate() { }

    public void OnFixedUpdate()
    {
        _sheathingState.OnFixedUpdate();
        
        float diffrence = GetRotationDiffrence();
        
        Debug.Log("angel diffrence:" + diffrence);
        
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
        _kM.Sheath.Vibrate(_vibrateAmplitude, 0.5f);
    }
    private float GetRotationDiffrence()
    {
        Quaternion current = _kM.Katana.HandRotation;
        
        Quaternion delta = current * Quaternion.Inverse(_orginalRotation);
        
        delta.ToAngleAxis(out float angle, out Vector3 axis);
        
        if (angle > 180f) angle -= 360f;
        
        float sign = Mathf.Sign(Vector3.Dot(axis, Vector3.up));

        return angle * sign;
    }
}