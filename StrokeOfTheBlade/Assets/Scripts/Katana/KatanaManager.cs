using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class KatanaManager : MonoBehaviour
{
    [SerializeField] private Katana katana;
    [SerializeField] private Sheath sheath;
    //private float _maxAngle = 40f;
    
    public Katana Katana => katana;
    public Sheath Sheath => sheath;
    
    private void OnEnable()
    {
        RefreshDevices();
        InputDevices.deviceConnected += OnDeviceConnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= OnDeviceConnected;
    }

    private void OnDeviceConnected(InputDevice device)
    {
        RefreshDevices();
    }

    private void RefreshDevices()
    {
        katana.RefreshDevice();
        sheath.RefreshDevice();
    }

    public bool IsTipInMouth() => sheath.MouthCheck.IsTagInside;
    
    public bool IsTipInEnd() => sheath.EndCheck.IsTagInside;
    
    /*
    public bool AreControllersAligned()
    {
        Vector3 axis = Vector3.forward;

        float angle = Vector3.SignedAngle(
            Katana.HandRotation * Vector3.up,
            Sheath.HandRotation * Vector3.up,
            axis
        );

        return Mathf.Abs(angle) <= _maxAngle;
    }*/
}
