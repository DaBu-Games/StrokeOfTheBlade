using System;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerColliderCheck : MonoBehaviour
{
    [SerializeField] private string targetTag = "KatanaTip";
    [SerializeField] private int _insideCount = 0;
    private BoxCollider[] _colliders;
    
    public BoxCollider[] Colliders => _colliders;

    public bool IsTagInside => _insideCount > 0;

    private void Start()
    {
        _colliders = this.GetComponentsInChildren<BoxCollider>();

        foreach (var boxCollider in _colliders)
        {
            boxCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
            _insideCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
            _insideCount--;
    }
}
