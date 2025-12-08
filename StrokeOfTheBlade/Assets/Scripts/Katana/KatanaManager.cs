using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class KatanaManager : MonoBehaviour
{
    [SerializeField] private Katana katana;
    [SerializeField] private Sheath sheath;
    
    [Header("Distances")]
    [SerializeField] private float sheathStartDist = 0.02f;
    
    [Header("Sheath Alignment")]
    [SerializeField, Range(0f, 1f)] private float alignmentThreshold = 0.8f;
    
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

    public bool IsTipNearMouth() => Vector3.Distance(katana.Tip.position, sheath.Mouth.position) < sheathStartDist;

    public bool IsSheating()
    {
        return sheath.InsideSheathCount > 0; 
    }
    
    public bool ForcedExitSheating()
    {
        Vector3 sheathAxis = (sheath.End.position - sheath.Mouth.position).normalized;
        Vector3 tipDir = (katana.Tip.position - sheath.Mouth.position).normalized;
        float alignment = Vector3.Dot(sheathAxis, tipDir);

        return alignment < alignmentThreshold;
    }
}
