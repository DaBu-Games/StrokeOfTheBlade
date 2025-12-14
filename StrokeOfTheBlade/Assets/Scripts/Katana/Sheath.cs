using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Sheath : BaseController
{
    [FormerlySerializedAs("_colliders")]
    [Header("Box colliders")] 
    [SerializeField] private OnTriggerColliderCheck collidersCheck;
    [SerializeField] private OnTriggerColliderCheck mouthCheck;
    [SerializeField] private OnTriggerColliderCheck endCheck;
    [SerializeField] private Transform pointsParent;
    
    private List<Transform> _points = new List<Transform>();

    protected override void OnAwake()
    {
        foreach (Transform child in pointsParent)
        {
            _points.Add(child);
        }
        
        Debug.Log(Rb);
    }

    public Rigidbody SRb => Rb;
    public OnTriggerColliderCheck CollidersCheck => collidersCheck;
    public OnTriggerColliderCheck MouthCheck => mouthCheck;
    public OnTriggerColliderCheck EndCheck => endCheck;
    
    public List<Transform> Points => _points;
}
