using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_Manager : MonoBehaviour
{
    [SerializeField] private Enemy_Behavior _enemyPrefab;
    [SerializeField] private bool _isShowGizmo;
    [SerializeField] private UI_Manager _ui;
    public Transform[] playerLocations;
    public Player_Behavior[] playerBehaviors;
    

    [Header("Pooling system components")]
    [Tooltip("The amount of enemy spawned when game start")][SerializeField] private int _spawnPresetAmount;
    [Tooltip("The max amount of enemy can be spawn during gameplay")][SerializeField]private int _spawnMaxAmount;
    [SerializeField] private float _spawnRange;
    [SerializeField] private float _spawnDelay;
    

    private int _firstWaveEnemyAmount = 10;
    private int _enemyAmountIncreaseRate = 2;
    private float _spawnDelayDecreaseRate = 0.1f;


    private IObjectPool<Enemy_Behavior> _enemyPool;
    private int _waveSpawnAmount, _waveCount = 0;
    private int _spawnCount;
    public int _enemyAliveCount { get; private set; }
    private float _difficulty;
    private Transform _transform;


    /* Monobehavior methods */
    private void Awake()
    {
        _transform = transform;
        _enemyPool = new ObjectPool<Enemy_Behavior>(OnCreateNewEnemy, OnGetEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, true, _spawnPresetAmount, _spawnMaxAmount);
        _waveSpawnAmount += _firstWaveEnemyAmount - _enemyAmountIncreaseRate;
        Invoke("NextWave", 3f);
    }


    /* Spawn methods */
    private void NextWave()
    {
        _waveCount++;
        _waveSpawnAmount += _enemyAmountIncreaseRate;
        _enemyAliveCount = _waveSpawnAmount;
        _spawnCount = 0;
        _difficulty ++;
        _ui.UpdateWaveCount(_waveCount);
        _ui.UpdateDuckCount(_enemyAliveCount);
        StartCoroutine(SpawnEnemyCoroutine());
    }
    private void TransitionToNextWave()
    {
        _spawnDelay -= _spawnDelayDecreaseRate;
        Invoke("NextWave", 3f);
    }
    IEnumerator SpawnEnemyCoroutine()
    {
        yield return new WaitForSeconds(4f);
        while (_spawnCount < _waveSpawnAmount)
        {
            Enemy_Behavior newEnemy = _enemyPool.Get(); // Spawn / Reactivate enemy
            yield return new WaitForSeconds(_spawnDelay);
        }
    }


    /* Pooling system methods */
    private Enemy_Behavior OnCreateNewEnemy()
    {
        _spawnCount++;
        Enemy_Behavior enemy = Instantiate(_enemyPrefab, transform.position, transform.rotation);
        enemy.ObjectPool = _enemyPool;
        enemy.SetUpEnemy(this);
        enemy.SetDifficulty((int)_difficulty);
        enemy.SetNewPosition(GetRandomPosition(), GetRandomScaleX());
        return enemy;
    }
    private void OnGetEnemyFromPool(Enemy_Behavior enemy)
    {
        if (enemy.gameObject.activeInHierarchy) return;

        _spawnCount++;
        enemy.SetDifficulty((int)_difficulty);
        enemy.SetNewPosition(GetRandomPosition(), GetRandomScaleX());
        enemy.ReactivateEnemy();
    }
    public void OnReturnEnemyToPool(Enemy_Behavior enemy)
    {
        _enemyAliveCount--;
        _ui.UpdateDuckCount(_enemyAliveCount);
        enemy.gameObject.SetActive(false);
        if (_enemyAliveCount == 0) TransitionToNextWave();
    }
    private void OnDestroyEnemy(Enemy_Behavior enemy)
    {
        Destroy(enemy.gameObject);
    }


    /* Other methods */
    private Vector2 GetRandomPosition() // Pick a random position in range
    {
        float randomPosX = Random.Range(_transform.position.x - _spawnRange / 2, transform.position.x + _spawnRange / 2);
        return new Vector2(randomPosX, _transform.position.y);
    }
    private Vector3 GetRandomScaleX()   // Pick random facing direction
    {
        int randomScaleX = Random.Range(0, 2);
        return new Vector3(randomScaleX == 0 ? 1 : -1, 1, 1);
    }
}
