using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Katana : BaseController
{
    [Header("katana value's")]
    [SerializeField] private Transform _tip;
    [SerializeField] private bool _isCharged;
    
    public Transform Tip => _tip;
    
    public void SetCharged(bool isCharged) => _isCharged = isCharged;
    public bool IsCharged => _isCharged;
}