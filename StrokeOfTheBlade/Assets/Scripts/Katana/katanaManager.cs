using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

public class katanaManager : MonoBehaviour
{
    [Header("RigidBody's")]
    [SerializeField] private Rigidbody katanaRb;
    
    [Header("Transforms")]
    [SerializeField] private Transform katanaTip;
    [SerializeField] private Transform sheathMouth;

    [Header("Controllers")]
    [SerializeField] private ActionBasedController  katana;
    [SerializeField] private ActionBasedController  sheath;
    
    [Header("Distances")]
    [SerializeField] private float maxBounds = 0.08f;   
    [SerializeField] private float pullStartDist = 0.04f;

    public bool IsTipNearMouth() => Vector3.Distance(katanaTip.position, sheathMouth.position) < pullStartDist;

    public bool IsTipTooFar() => Vector3.Distance(katanaTip.position, sheathMouth.position) > maxBounds;

    public float DistanceToMouth() => Vector3.Distance(katanaTip.position, sheathMouth.position);

    public void VibrateKatana(float amplitude, float duration)
    {
        katana.SendHapticImpulse(amplitude, duration);
    }
    
    public void VibrateSheath(float amplitude, float duration)
    {
        sheath.SendHapticImpulse(amplitude, duration);
    }
}
