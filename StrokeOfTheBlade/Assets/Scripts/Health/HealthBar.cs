using System;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private Entity _entity;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = _maxHealth;
        _entity = GetComponent<Entity>();
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        
        _entity.OnHit();

        if (_currentHealth <= 0)
        {
            _entity.OnDestroy();
        }
    }
    
}