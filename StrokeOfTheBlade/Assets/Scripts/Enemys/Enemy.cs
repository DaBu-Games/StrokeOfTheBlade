using System;
using UnityEngine;

public class Enemy : MonoBehaviour, Entity
{
    [SerializeField] private BaseElement _element;
    [SerializeField] private float _fireRate;

    public void OnDestroy()
    {
        Destroy(gameObject);
    }
}