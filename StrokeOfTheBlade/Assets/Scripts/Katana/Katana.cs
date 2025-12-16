using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Katana : BaseController
{
    [SerializeField] private Transform _tip;
    
    public Transform Tip => _tip;
}