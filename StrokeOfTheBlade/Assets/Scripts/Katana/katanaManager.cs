using UnityEngine;
using UnityEngine.XR;

public class katanaManager : MonoBehaviour
{
    [Header("RigidBody's")]
    [SerializeField] private Rigidbody katanaRb;
    
    [Header("Transforms")]
    [SerializeField] private Transform katanaTip;
    [SerializeField] private Transform sheathMouth;
    [SerializeField] private Transform sheathEnd;

    [Header("Controllers")]
    [SerializeField] private XRNode katana;
    [SerializeField] private XRNode sheath;
    
    private InputDevice _katanaDevice;
    private InputDevice _sheathDevice;
    
    [Header("Distances")]
    [SerializeField] private float maxBounds = 0.08f;   
    [SerializeField] private float pullStartDist = 0.04f;
    
    [Header("Sheath Alignment")]
    [SerializeField, Range(0f, 1f)] private float alignmentThreshold = 0.8f;
    
    public Rigidbody KatanaRb => katanaRb;
    public Transform KatanaTip => katanaTip;
    public Transform SheathMouth => sheathMouth;
    public Transform SheathEnd => sheathEnd;
    
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
        _katanaDevice = InputDevices.GetDeviceAtXRNode(katana);
        _sheathDevice = InputDevices.GetDeviceAtXRNode(sheath);
    }
    
    public void VibrateKatana(float amplitude, float duration)
    {
        if (_katanaDevice.isValid && _katanaDevice.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
        {
            _katanaDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
    
    public void VibrateSheath(float amplitude, float duration)
    {
        if (_sheathDevice.isValid && _sheathDevice.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
        {
            _sheathDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }

    public bool IsTipNearMouth() => Vector3.Distance(katanaTip.position, sheathMouth.position) < pullStartDist;

    public bool IsSheating()
    {
        return 
    }
    
    public bool ForcedExitSheating()
    {
        Vector3 sheathAxis = (sheathEnd.position - sheathMouth.position).normalized;
        Vector3 tipDir = (katanaTip.position - sheathMouth.position).normalized;
        float alignment = Vector3.Dot(sheathAxis, tipDir);

        return alignment < alignmentThreshold;
    }
}
