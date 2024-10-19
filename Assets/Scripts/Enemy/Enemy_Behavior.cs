using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy_Behavior : MonoBehaviour
{
    [SerializeField] Enemy_Stat _stat;

    [Header("Ignore")]
    private IObjectPool<Enemy_Behavior> _objectPool;
    public IObjectPool<Enemy_Behavior> ObjectPool { set => _objectPool = value; }


    private Transform _transform;
    private int _health, _difficulty;
    private float _moveSpeed, _moveSpeedIncreaseBy = 0.1f;
    private bool _isCanDoThings, isAlive;

    private void Awake()
    {
        _transform = transform;
        ResetComponents();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_isCanDoThings)
            _isCanDoThings = true;  // Initial check for first ground touch after spawn
    }

    public void TakeDamage(int value)
    {
        _health += value;
        /* Implement hurt mechanic */

        if (_health <= 0)
        {
            /* Implement dead transition effect (Explosion or something) */
            DieEffect();    
            DeactivateEnemy();
        }
    }
    private void DieEffect()
    {
        isAlive = false;
    }


    /* Pooling system methods */
    public void DeactivateEnemy()
    { 
        this.gameObject.SetActive(false);
    }
    public void ReactivateEnemy()
    {
        ResetComponents();
        this.gameObject.SetActive(true);
    }

    /* Helper methods */
    public void SetNewPosition(Vector2 newPos, Vector3 newScaleX)
    { 
        _transform.position = newPos;
        _transform.localScale = newScaleX;
    }
    private void ResetComponents()
    {
        _health = _stat._maxHealth + _difficulty;
        _isCanDoThings = false;
        isAlive = true;
    }
    public void SetDifficulty(int value)
    {
        _difficulty = value;
        _moveSpeed = _stat._moveSpeed + value * _moveSpeedIncreaseBy;
    }
}
