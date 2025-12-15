using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Sheath : BaseController
{
    [FormerlySerializedAs("_colliders")]
    [Header("Box colliders")] 
    [SerializeField] private OnTriggerColliderCheck mouthCheck;
    [SerializeField] private OnTriggerColliderCheck endCheck;
    
    public OnTriggerColliderCheck MouthCheck => mouthCheck;
    public OnTriggerColliderCheck EndCheck => endCheck;
}
