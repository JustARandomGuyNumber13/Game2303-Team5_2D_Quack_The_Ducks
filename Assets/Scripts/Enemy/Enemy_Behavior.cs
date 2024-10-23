using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Pathfinding;
using Unity.VisualScripting;

public class Enemy_Behavior : MonoBehaviour
{
    [SerializeField] Enemy_Stat _stat;

    [Header("Ignore")]
    private IObjectPool<Enemy_Behavior> _objectPool;
    public IObjectPool<Enemy_Behavior> ObjectPool { set => _objectPool = value; }

    [Header("Pathfinding")]
    private float _nextWayPointDistance = 1f;
    private Path _path;
    private Seeker _seeker;
    private int _currentWayPoint;
    private bool _isReachedEndOfPath;


    private Transform[] _playerLocations;
    private Transform _transform, _targetTransform;
    private Enemy_Manager _enemyManager;
    private Rigidbody2D _rb;
    private Collider2D _collider;
    private int _health, _difficulty;
    private float _moveSpeed;
    private bool _isCanDoThings, _isAlive, isOnGround;

    private void Awake()
    {
        _transform = transform;
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _seeker = GetComponent<Seeker>();
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

        // Check is on ground
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, _stat._groundCheckDistance, LayerMask.GetMask("Ground"));
        if (hit.collider != null) isOnGround = true;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * _stat._groundCheckDistance);
    }
    private void FixedUpdate()
    {
        if (_isAlive && _isCanDoThings)
            PathFollow();
    }


    /* Chase target handlers */
    private IEnumerator ChaseCoroutine()
    {
        while (_isAlive)
        {
            FindClosestPlayer();
            ChasePlayer();
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void ChasePlayer()
    {
        UpdatePath();
    }
    private void UpdatePath()
    {
        if(_seeker.IsDone()) 
            _seeker.StartPath(_transform.position, _targetTransform.position, OnPathComplete);      // MOVE
    }
    private void PathFollow()
    {
        if (_path == null) return;

        if (_currentWayPoint >= _path.vectorPath.Count)
        {
            _isReachedEndOfPath = true;
            return;
        }
        else _isReachedEndOfPath = false;


        Vector2 direction = (_path.vectorPath[_currentWayPoint] - _transform.position).normalized;
        Vector2 moveForce = direction.x * _stat._moveSpeed * Vector2.right * Time.fixedDeltaTime;
        _rb.AddForce(moveForce, ForceMode2D.Force);


        if (isOnGround)       
        {
            Vector3 tarPos = _targetTransform.position;
            Vector3 pathPos = _path.vectorPath[_currentWayPoint];
            Vector3 curPos = _transform.position;

            if (tarPos.y < curPos.y && Mathf.Abs(tarPos.x - curPos.x) < 2f)
                Descend();
            else if (curPos.y - pathPos.y > _stat._jumpNodeHeightRequirement)
                Descend();
            else if (pathPos.y - curPos.y > _stat._jumpNodeHeightRequirement && tarPos.y > curPos.y)    // Ascend / Jump
                Ascend();
        }

        float distance = Vector2.Distance(_transform.position, _path.vectorPath[_currentWayPoint]);
        if (distance < _nextWayPointDistance)
            _currentWayPoint++;

        FaceDirection();
    }
    private void Ascend()
    {
        if (!_collider.usedByEffector)
        {
            _rb.AddForce(Vector2.up * _stat._jumpForce, ForceMode2D.Impulse);
            isOnGround = false;
        }
    }
    private void Descend()
    {
        _collider.isTrigger = true;
        isOnGround = false;
        Invoke("ResetDescend", 0.3f);
    }
    private void ResetDescend()
    {
        _collider.isTrigger = false;
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWayPoint = 0;
        }
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
    private void FaceDirection()
    {
        Vector3 curScale = _transform.localScale;
        curScale.x = _rb.velocity.x > 0 ? 1 : -1;
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
        Vector3 bounceBackDir = _transform.position - other.position;
        _rb.velocity = Vector2.up * _rb.velocity.y; // Reset velocity x
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
        _collider.enabled = false;
        _rb.gravityScale = 0;
        _enemyManager.OnReturnEnemyToPool(this);
        this.gameObject.SetActive(false);
    }
    public void ReactivateEnemy()
    {
        _collider.enabled = true;
        _rb.gravityScale = 1;
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
