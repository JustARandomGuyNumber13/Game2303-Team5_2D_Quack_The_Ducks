using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Pathfinding;
using Unity.VisualScripting;

public class Enemy_Behavior : MonoBehaviour
{
    [SerializeField] Enemy_Stat _stat;
    [SerializeField] private GameObject _explosionSprite, _enemySprite;


    [Header("Pathfinding")]
    private float _nextWayPointDistance = 1f;
    private Path _path;
    private Seeker _seeker;
    private int _currentWayPoint;


    private IObjectPool<Enemy_Behavior> _objectPool;
    public IObjectPool<Enemy_Behavior> ObjectPool { set => _objectPool = value; }
    private Transform[] _playerLocations;
    private Player_Behavior[] _playerBehaviors;
    private Transform _transform, _targetTransform;
    private Rigidbody2D _rb;
    private Collider2D _collider, _curPlatformStandOn;
    private int _health, _difficulty;
    private float _moveSpeed;
    private bool _isCanDoThings, _isAlive, isOnGround;


    /* Monobehavior methods */
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
        // Initial check for first ground touch after spawn
        if (!_isCanDoThings)
        {
            _isCanDoThings = true;  
            StartCoroutine(ChaseCoroutine());
        }

        // Die on touch
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<Player_Behavior>().TakeDamage(-1, _transform);
            DieEffect();
            return;
        }
        else if (collision.gameObject.tag == "Destroyer")
        {
            DieEffect();
            return;
        }

        // Check is on ground
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, _stat._groundCheckDistance, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            _curPlatformStandOn = hit.collider;
            isOnGround = true;
        }
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
            UpdatePath();
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void UpdatePath()
    {
        if(_seeker.IsDone() && _targetTransform != null) 
            _seeker.StartPath(_transform.position, _targetTransform.position, OnPathComplete);      
    }
    private void PathFollow()
    {
        if (_path == null || _currentWayPoint >= _path.vectorPath.Count)
            return;
        
        // Movement calculation handler
        Vector2 direction = (_path.vectorPath[_currentWayPoint] - _transform.position).normalized;
        Vector2 moveForce = direction.x * _stat._moveSpeed * Vector2.right * Time.fixedDeltaTime;
        _rb.AddForce(moveForce, ForceMode2D.Force);    

        // Ascend and Descend handler
        if (isOnGround)      
        {
            Vector3 tarPos = _targetTransform.position;
            Vector3 pathPos = _path.vectorPath[_currentWayPoint];
            Vector3 curPos = _transform.position;

            if (tarPos.y < curPos.y && Mathf.Abs(tarPos.x - curPos.x) < 2f)
                Descend();
            else if (curPos.y - pathPos.y > _stat._jumpNodeHeightRequirement)
                Descend();
            else if (pathPos.y - curPos.y > _stat._jumpNodeHeightRequirement && tarPos.y > curPos.y)    
                Ascend();
        }

        // Update next destination
        float distance = Vector2.Distance(_transform.position, _path.vectorPath[_currentWayPoint]);
        if (distance < _nextWayPointDistance)
            _currentWayPoint++;

        if(_targetTransform.position.y < -3)
            _rb.velocity = Vector2.up * _rb.velocity.y;

        FaceDirection();
    }
    private void Ascend()
    {
        _rb.velocity = Vector2.right * _rb.velocity.x;
        _rb.AddForce(Vector2.up * _stat._jumpForce, ForceMode2D.Impulse);
        isOnGround = false;
    }
    private void Descend()
    {
        Physics2D.IgnoreCollision(_collider, _curPlatformStandOn, true);
        isOnGround = false;
        Invoke("ResetDescend", 0.3f);
    }
    private void ResetDescend()
    {
        Physics2D.IgnoreCollision(_collider, _curPlatformStandOn, false);
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
        _transform.localScale = curScale;
    }


    /* Health handlers */
    public void TakeDamage(int value, Transform other)
    {
        _health += value;
        HurtEffect(other);   

        if (_health <= 0)
        {
            DieEffect();  
        }
    }
    private void HurtEffect(Transform other)
    { 
        // Push enemy back against "other" direction
        Vector3 bounceBackDir = _transform.position - other.position;
        _rb.velocity = Vector2.up * _rb.velocity.y; // Reset velocity x
        _rb.AddForce(bounceBackDir * _stat._bounceBackForce, ForceMode2D.Impulse);
    }
    private void DieEffect()
    {
        // Cause a small explosion toward every "target" in the area
        Collider2D[] hit = Physics2D.OverlapCircleAll(_transform.position, _stat._explosionRange, LayerMask.GetMask("Player"));
        if (hit.Length > 0)
        {
            foreach (Collider2D target in hit)
                for (int i = 0; i < _playerLocations.Length; i++)
                    if (target.gameObject == _playerLocations[i].gameObject)
                    {
                        _playerBehaviors[i].TakeDamage(0, _transform);
                    }
        }
        _rb.velocity = Vector2.right;
        FaceDirection();
        _enemySprite.SetActive(false);
        _explosionSprite.SetActive(true);
        _isAlive = false;
        _collider.enabled = false;
        _rb.velocity = Vector2.zero;
        _rb.gravityScale = 0;
        Invoke("DeactivateEnemy", 0.5f);
    }


    /* Pooling system methods */
    public void DeactivateEnemy()
    {
        _objectPool.Release(this);
    }
    public void ReactivateEnemy()
    {
        _enemySprite.SetActive(true);
        _explosionSprite.SetActive(false);
        _collider.enabled = true;
        _rb.gravityScale = 3;
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
        _health = _stat._maxHealth + _difficulty / 3;
        _isCanDoThings = false;
        _isAlive = true;
    }
    public void SetDifficulty(int value)
    {
        _difficulty = value;
        _moveSpeed = _stat._moveSpeed + _difficulty * _stat._moveSpeedIncreaseBy;
    }


    /* Other methods */
    public void SetUpEnemy(Enemy_Manager value)
    {
        _playerLocations = value.playerLocations;
        _playerBehaviors = value.playerBehaviors;
    }
}
