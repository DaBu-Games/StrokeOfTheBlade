using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Katana : MonoBehaviour
{
    [Header("RigidBody's")]
    [SerializeField] private Rigidbody rb;
    
    [Header("Transforms")]
    [SerializeField] private Transform tip;
    
    [Header("Controller")]
    [SerializeField] private XRNode katana;
    private InputDevice _katanaDevice;
    
    public Rigidbody Rb => rb;
    public Transform Tip => tip;

    public void RefreshDevice()
    {
        _katanaDevice = InputDevices.GetDeviceAtXRNode(katana);
    }
    
    public void Vibrate(float amplitude, float duration)
    {
        if (_katanaDevice.isValid && _katanaDevice.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
        {
            _katanaDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
}