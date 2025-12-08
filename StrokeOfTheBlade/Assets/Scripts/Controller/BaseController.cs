using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

[RequireComponent(typeof(Rigidbody))]
public class BaseController : MonoBehaviour
{
    [SerializeField] private XRNode controllerNode;
    private InputDevice _controllerDevice;
    
    [Header("Follow Settings")]
    [SerializeField] private bool followController = true;
    
    [Header("Position Follow")]
    [SerializeField] private float positionLerp = 30f;

    [Header("Rotation Follow")]
    [SerializeField] private float rotationSmoothFactor = 0.2f; 
    [SerializeField] private float maxAngularSpeed = 40f;
    
    protected Rigidbody Rb;
    
    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        
        Rb.isKinematic = false;
        Rb.useGravity = false;
        Rb.interpolation = RigidbodyInterpolation.Interpolate;
        Rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        RefreshDevice();
    }
    
    private void FixedUpdate()
    {
        if (!followController) return;

        if (!_controllerDevice.isValid ||
            !_controllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 handPos) ||
            !_controllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion handRot))
            return;

        FollowPosition(handPos);
        FollowRotation(handRot);
    }
    
    private void FollowPosition(Vector3 targetPos)
    {
        Vector3 delta = targetPos - Rb.position;
        Vector3 velocity = delta * positionLerp;

        Rb.linearVelocity = velocity;
    }
    
    private void FollowRotation(Quaternion targetRot)
    {
        Quaternion rotDelta = targetRot * Quaternion.Inverse(Rb.rotation);
        rotDelta.ToAngleAxis(out float angle, out Vector3 axis);
        
        if (float.IsNaN(axis.x) || float.IsInfinity(axis.x))
            return;
        
        if (angle > 180f) angle -= 360f;
        
        angle *= rotationSmoothFactor;
        
        Vector3 angularVel = axis.normalized * (angle * Mathf.Deg2Rad) / Time.fixedDeltaTime;
        
        angularVel = Vector3.ClampMagnitude(angularVel, maxAngularSpeed);

        Rb.angularVelocity = angularVel;
    }

    public void RefreshDevice()
    {
        _controllerDevice = InputDevices.GetDeviceAtXRNode(controllerNode);
    }
    
    public void Vibrate(float amplitude, float duration)
    {
        if (_controllerDevice.isValid && _controllerDevice.TryGetHapticCapabilities(out HapticCapabilities caps) && caps.supportsImpulse)
        {
            _controllerDevice.SendHapticImpulse(0, amplitude, duration);
        }
    }
        
}