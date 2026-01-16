using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

[RequireComponent(typeof(Rigidbody))]
public class BaseController : MonoBehaviour
{
    [SerializeField] private XRNode controllerNode;
    
    [Header("Input Actions")]
    [SerializeField] private List<InputActionEntry> inputActions = new List<InputActionEntry>();
    
    private InputDevice _controllerDevice;
    
    [Header("Follow Settings")]
    [SerializeField] private bool followController = true;
    
    [Header("Position Follow")]
    [SerializeField] private float positionLerp = 30f;

    [Header("Rotation Follow")]
    [SerializeField] private float rotationSmoothFactor = 0.2f; 
    [SerializeField] private float maxAngularSpeed = 40f;

    private Vector3 _handPos;
    private Quaternion _handRot;
    
    private Rigidbody _rb;
    
    private Dictionary<string, InputAction> _actionLookup;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        
        _rb.isKinematic = false;
        _rb.useGravity = false;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        _actionLookup = new Dictionary<string, InputAction>();

        foreach (var entry in inputActions)
        {
            if (entry.action != null)
            {
                _actionLookup[entry.key] = entry.action.action;
            }
        }
        
        RefreshDevice();
    }
    
    public void FixedUpdate()
    {
        if (!_controllerDevice.TryGetFeatureValue(CommonUsages.devicePosition, out _handPos) ||
            !_controllerDevice.TryGetFeatureValue(CommonUsages.deviceRotation, out _handRot))
            return;
        
        if (!followController || !_controllerDevice.isValid) return;

        FollowPosition(_handPos);
        FollowRotation(_handRot);
    }
    
    public void FollowController(bool follow) => followController = follow;
    
    public Vector3 HandPosition => _handPos;
    public Rigidbody Rb => _rb;
    public Quaternion HandRotation => _handRot;
    public XRNode ControllerNode => controllerNode;

    public InputAction GetAction(string actionName)
    {
        return _actionLookup[actionName];
    }

    public void SetDevice(XRNode node)
    {
        controllerNode = node;
        RefreshDevice();
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
    
    private void FollowPosition(Vector3 targetPos)
    {
        Vector3 delta = targetPos - _rb.position;
        Vector3 velocity = delta * positionLerp;

        _rb.linearVelocity = velocity;
    }
    
    private void FollowRotation(Quaternion targetRot)
    {
        Quaternion rotDelta = targetRot * Quaternion.Inverse(_rb.rotation);
        rotDelta.ToAngleAxis(out float angle, out Vector3 axis);
        
        if (float.IsNaN(axis.x) || float.IsInfinity(axis.x))
            return;
        
        if (angle > 180f) angle -= 360f;
        
        angle *= rotationSmoothFactor;
        
        Vector3 angularVel = axis.normalized * (angle * Mathf.Deg2Rad) / Time.fixedDeltaTime;
        
        angularVel = Vector3.ClampMagnitude(angularVel, maxAngularSpeed);

        _rb.angularVelocity = angularVel;
    }
        
}