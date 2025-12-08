using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Sheath : MonoBehaviour
{
    [Header("Transforms")]
    [SerializeField] private Transform mouth;
    [SerializeField] private Transform end;
    
    [Header("Controller")]
    [SerializeField] private XRNode sheath;
    private InputDevice _sheathDevice;

    private int _insideSheathCount = 0;
    
    public int InsideSheathCount => _insideSheathCount;
    public Transform Mouth => mouth;
    public Transform End => end;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("KatanaTip"))
            _insideSheathCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KatanaTip") && _insideSheathCount > 0)
            _insideSheathCount--;
    }
    
    public void RefreshDevice()
    {
        _sheathDevice = InputDevices.GetDeviceAtXRNode(sheath);
    }
    
    public void Vibrate(float amplitude, float duration)
    {
        if (_sheathDevice.isValid && _sheathDevice.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
        {
            _sheathDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
}
