using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, Entity
{
    public static event Action<Enemy> OnEnemyDestroyed;
    
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _rotationSpeed = 5f;

    [Header("projectile")] 
    [SerializeField] private float _projectileLength = 0.6f;
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _firePoint;
    
    private Vector3 _target;
    private BaseElement _element;

    public void OnDestroy()
    {
        OnEnemyDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    public void Initialize(Vector3 target, BaseElement element)
    {
        _target = target;
        _element = element;
        transform.rotation = GetTargetRotation();
        _meshRenderer.material = _element.Data.Material;
    }

    private void Update()
    {
        RotateTowardsTarget();
    }

    private void RotateTowardsTarget()
    {
        Quaternion targetRotation = GetTargetRotation();
        
        if (targetRotation == Quaternion.identity)
            return;
       
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            _rotationSpeed * Time.deltaTime
        );
    }

    private Quaternion GetTargetRotation()
    { 
        Vector3 direction = _target - transform.position;
        direction.y = 0;
        
        if (direction.sqrMagnitude < 0.01f)
            return Quaternion.identity;
        
        return Quaternion.LookRotation(direction, Vector3.up);
    }

    public void Shoot()
    {
        GameObject projectile = Instantiate(_projectile, _firePoint.position, _firePoint.rotation);
        
        float speed = Random.Range(_element.Data.MinProjectileSpeed, _element.Data.MaxProjectileSpeed);
        
        projectile.GetComponent<ElementalProjectile>().Initialize(_element, speed, GenerateProjectilePoints());
    }
    
    private List<Vector3> GenerateProjectilePoints()
    {
        bool useUp = Random.value > 0.5f;

        Vector3 sideDir = useUp ? _firePoint.up : _firePoint.right;

        Vector3 start = _firePoint.position - sideDir * _projectileLength;
        Vector3 end   = _firePoint.position + sideDir * _projectileLength;

        return new List<Vector3>
        {
            start,
            end
        };
    }
}