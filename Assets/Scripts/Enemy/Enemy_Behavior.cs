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


    private Transform[] _playerLocations;
    private Transform _transform, _targetTransform;
    private Enemy_Manager _enemyManager;
    private Rigidbody2D _rb;
    private int _health, _difficulty;
    private float _moveSpeed;
    private bool _isCanDoThings, _isAlive, isOnGround;

    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        ResetComponents();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!_isCanDoThings)
        {
            _isCanDoThings = true;  // Initial check for first ground touch after spawn
            StartCoroutine(ChaseCoroutine());
        }

        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player_Behavior>().TakeDamage(-1, _transform);
            DieEffect();    // Not implement yet
            DeactivateEnemy();
            return;
        }

        RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, 0.2f, LayerMask.GetMask("Ground"));
        if(hit.collider != null) isOnGround = true;
    }

    /* Chase target handlers */
    private IEnumerator ChaseCoroutine()
    {
        while (_isAlive)
        {
            FindClosestPlayer();
            FaceCharacterDirection();
            ChasePlayer();
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void ChasePlayer()
    {
        Vector2 direction = _targetTransform.position - _transform.position;
    }
    private void Jump()
    {
        if(isOnGround)
            _rb.AddForce(Vector2.up * _stat._jumpForce, ForceMode2D.Impulse);
    }
    private void FindClosestPlayer()
    {
        float distance = 1000f;
        foreach (Transform player in _playerLocations)
        {
            if (!player.gameObject.activeInHierarchy) continue;
            float distToPlayer = Vector3.Distance(_transform.position, player.position);
            if (distToPlayer < distance)
            {
                distance = distToPlayer;
                _targetTransform = player;
            }
        }
    }
    private void FaceCharacterDirection()
    {
        Vector3 curScale = _transform.localScale;
        curScale.x = _targetTransform.position.x > _transform.position.x ? 1 : -1;
        _transform.localScale.Scale(curScale);
    }


    /* Health handlers */
    public void TakeDamage(int value, Transform other)
    {
        _health += value;
        HurtEffect(other);   // Not implement yet

        if (_health <= 0)
        {
            DieEffect();    // Not implement yet
            DeactivateEnemy();
        }
    }
    private void HurtEffect(Transform other)
    { 
        /* Implement hurt effect */
        Vector3 bounceBackDir = _transform.position - other.position;
        _rb.AddForce(bounceBackDir * _stat._bounceBackForce, ForceMode2D.Impulse);
    }
    private void DieEffect()
    {
        /* Implement die effect */
        _isAlive = false;
    }


    /* Pooling system methods */
    public void DeactivateEnemy()
    {
        _enemyManager.OnReturnEnemyToPool(this);
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
        _isAlive = true;
    }
    public void SetDifficulty(int value)
    {
        _difficulty = value;
        _moveSpeed = _stat._moveSpeed + _difficulty * _stat._moveSpeedIncreaseBy;
    }


    /* Other methods */
    public void SetUpEnemy(Transform[] value1, Enemy_Manager value2)
    { 
        _playerLocations = value1;
        _enemyManager = value2;
    }
}
