using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

public class Sheath : BaseController
{
    [Header("Box colliders")] 
    [SerializeField] private OnTriggerColliderCheck mouthCheck;
    [SerializeField] private OnTriggerColliderCheck endCheck;
    [Header("Animation")] 
    [SerializeField] private Animation _animation;
    
    public OnTriggerColliderCheck MouthCheck => mouthCheck;
    public OnTriggerColliderCheck EndCheck => endCheck;
    public Animation Animation => _animation;
}
