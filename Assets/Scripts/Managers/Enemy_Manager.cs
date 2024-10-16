using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_Manager : MonoBehaviour
{
    [SerializeField] private Enemy_Behavior _enemy;
    [SerializeField] private IObjectPool<Enemy_Behavior> _enemyPool;
    [SerializeField] private int _spawnPresetAmount, _spawnMaxAmount;
    [SerializeField] private float _spawnRange;
    [SerializeField] private float _spawnDelay;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
        _enemyPool = new ObjectPool<Enemy_Behavior>(CreateNewEnemy, OnTakeEnemyFromPool, OnReturnEnemyToPool, OnDestroyEnemy, false, _spawnPresetAmount, _spawnMaxAmount);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_transform.position, Vector2.right * _spawnRange / 2);
        Gizmos.DrawLine(_transform.position, Vector2.left * _spawnRange / 2);
    }

    public void NextWave()
    { 
    
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        yield return new WaitForSeconds(_spawnDelay);
    }

    private Enemy_Behavior CreateNewEnemy()
    {
        Enemy_Behavior newEnemy = Instantiate(_enemy, transform.position, transform.rotation);
        return newEnemy;
    }
    private void OnTakeEnemyFromPool(Enemy_Behavior enemy)
    {
        enemy.ReactivateEnemy();
        enemy.SetNewPosition(GetRandomPosition(), GetRandomScaleX());
    }
    private void OnReturnEnemyToPool(Enemy_Behavior enemy)
    {
        enemy.DeactivateEnemy();
    }
    private void OnDestroyEnemy(Enemy_Behavior enemy)
    {
        Destroy(enemy);
    }
    private Vector2 GetRandomPosition()
    {
        float randomPosX = Random.Range(_transform.position.x - _spawnRange / 2, transform.position.x + _spawnRange / 2);
        return new Vector2(randomPosX, _transform.position.y);
    }
    private Vector3 GetRandomScaleX()
    {
        int randomScaleX = Random.Range(0, 2);
        return new Vector3(randomScaleX == 0 ? 1 : -1, 1, 1);
    }



}
