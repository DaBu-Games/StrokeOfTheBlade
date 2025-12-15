using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Sheath : BaseController
{
    [FormerlySerializedAs("_colliders")]
    [Header("Box colliders")] 
    [SerializeField] private OnTriggerColliderCheck colliderCheck;
    [SerializeField] private OnTriggerColliderCheck mouthCheck;
    [SerializeField] private OnTriggerColliderCheck endCheck;
    [SerializeField] private Animation _animation;
    
    public OnTriggerColliderCheck ColliderCheck => colliderCheck;
    public OnTriggerColliderCheck MouthCheck => mouthCheck;
    public OnTriggerColliderCheck EndCheck => endCheck;
    public Animation Animation => _animation;
}
