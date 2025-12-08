using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Sheath : BaseController
{
    [Header("Transforms")]
    [SerializeField] private Transform mouth;
    [SerializeField] private Transform end;

    private int _insideSheathCount = 0;
    
    public int InsideSheathCount => _insideSheathCount;
    public Transform Mouth => mouth;
    public Transform End => end;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("KatanaTip"))
            _insideSheathCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KatanaTip") && _insideSheathCount > 0)
            _insideSheathCount--;
    }
}
