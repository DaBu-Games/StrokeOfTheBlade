using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class KatanaManager : MonoBehaviour
{
    [SerializeField] private Katana katana;
    [SerializeField] private Sheath sheath;
    
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

    public bool IsTipNearMouth() => sheath.MouthCheck.IsTagInside;
    
    public bool IsTipNearEnd() => sheath.EndCheck.IsTagInside;

    public bool IsSheating() => Sheath.CollidersCheck.IsTagInside;
}
