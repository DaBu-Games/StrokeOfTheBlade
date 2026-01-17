using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _player;
    [SerializeField] private Elementmanager _elementManager;
    
    [Header("enemy spawn values")]
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _spawnPointsParent;
    [SerializeField] private int _maxEnemies = 8;
    [SerializeField] private int _minEnemies = 3;
    [SerializeField] private float _maxSpawnRate = 5f;
    [SerializeField] private float _minSpawnRate = 1f;
    
    [Header("Fire rate")]
    [SerializeField] private float _maxFireRate = 5f;
    [SerializeField] private float _minFireRate = 2f;
    
    private List<Transform> _spawnPoints;
    private float _spawnTimer = 0f;
    private float _spawnRate = 0f;
    
    private float _fireTimer = 0f;
    private float _fireRate = 0f;

    private List<Enemy> _currentEnemies = new List<Enemy>();
    private HashSet<Transform> _occupiedPoints = new HashSet<Transform>();
    
    private void OnEnable()
    {
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _currentEnemies.Remove(enemy);
    }

    private void Start()
    {
        SetFireRate();
        SetSpawnRate();
        _spawnPoints = _spawnPointsParent.GetComponentsInChildren<Transform>().ToList();
        _spawnPoints.RemoveAt(0);
    }

    private void Update()
    {
        if (Time.time - _fireTimer >= _fireRate && _currentEnemies.Count > 0)
        {
            Shoot();
            SetFireRate();
        }
        else if (_currentEnemies.Count == 0)
        {
            _fireTimer = Time.time;
        }
        
        if (_currentEnemies.Count < _maxEnemies && (Time.time - _spawnTimer >= _spawnRate || _currentEnemies.Count < _minEnemies))
        {
            SpawnEnemy();
            SetSpawnRate();
        }
        else if (_currentEnemies.Count == _maxEnemies)
        {
            _spawnTimer = Time.time;
        }
    }
    
    private void SetFireRate()
    {
        _fireTimer = Time.time;
        _fireRate = Random.Range(_minFireRate, _maxFireRate);
    }

    private void SetSpawnRate()
    {
        _spawnTimer = Time.time;
        _spawnRate = Random.Range(_minSpawnRate, _maxSpawnRate);
    }

    private void Shoot()
    {
        int index = Random.Range(0, _currentEnemies.Count);
        _currentEnemies[index].Shoot();
    }

    private void SpawnEnemy()
    {
        Transform spawnPoint = GetRandomSpawnPoint();
        if(spawnPoint == null)
            return;
        
        GameObject obj = Instantiate(_enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(_player, _elementManager.GetRandomElement());
        
        _currentEnemies.Add(enemy);
        _occupiedPoints.Add(spawnPoint);
    }

    private Transform GetRandomSpawnPoint()
    {
        if(_occupiedPoints.Count == _spawnPoints.Count)
            return null;
        
        List<Transform> freePoints = new List<Transform>();

        foreach (Transform p in _spawnPoints)
        {
            if(!_occupiedPoints.Contains(p))
                freePoints.Add(p);
        }
        
        return freePoints[Random.Range(0, freePoints.Count)];
    }
}
