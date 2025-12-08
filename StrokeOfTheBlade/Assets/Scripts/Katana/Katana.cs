using UnityEngine;
using UnityEngine.XR;

public class Katana : BaseController
{
    [Header("Transforms")]
    [SerializeField] private Transform tip;
    
    public Rigidbody KRb => Rb;
    public Transform Tip => tip;
}