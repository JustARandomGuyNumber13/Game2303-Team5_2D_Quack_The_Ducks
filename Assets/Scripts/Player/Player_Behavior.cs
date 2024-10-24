using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Behavior : MonoBehaviour
{
    [Header("Manual components")]
    [SerializeField] private Player_Manager _manager;
    [SerializeField] private UI_Manager _managerUI;

    [Header("Prefab components")]
    [SerializeField] private Player_Stat _stat;
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _shieldSprite;

    private Rigidbody2D _rb;
    private Transform _transform;
    private Collider2D _collider;

    private float _moveInput;
    private bool _isOnGround;
    private bool _isSecondJump;
    private bool _isCanDoThings = true;
    private float _groundCheckDistance = 1f;

    public int _health { get; private set; }

    private float _attackCoolDown;


    /* Monobehavior methods */
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = transform;
        _collider = this.GetComponent<Collider2D>();
        _health = _stat._maxHealth;
    }
    private void Update()
    {
        _attackCoolDown += Time.deltaTime;
        Move();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GroundCheck();
        if(collision.gameObject.tag == "Destroyer")
            DeactivatePlayer();
    }


    /* Movement handlers */
    private void Move()
    {
        float xSpeed = Mathf.MoveTowards(_rb.velocity.x, _stat._moveSpeed * _moveInput, _stat._accelerationSpeed * Time.fixedDeltaTime);
        _rb.velocity = Vector2.right * xSpeed + Vector2.up * _rb.velocity.y;
    }
    private void Jump()
    {
        if (_isOnGround || !_isSecondJump)
        {
            if (!_isOnGround) 
                _isSecondJump = true;

            _rb.velocity = _rb.velocity - _rb.velocity.y * Vector2.up;  // Reset y velocity
            _rb.AddForce(Vector2.up * _stat._jumpForce, ForceMode2D.Impulse);
            _anim.SetTrigger("jump");
            _anim.SetBool("onGround", false);
            _isOnGround = false;
        }
    }
    private void Descend()
    {
        if (_isOnGround && _transform.position.y > -2.7f)
        {
            _collider.isTrigger = true;
            _isOnGround = false;
            Invoke("DescendReset", 0.3f);
        }
    }
    private void DescendReset()
    {
        _collider.isTrigger = false;
    }
    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down, _groundCheckDistance, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            _isOnGround = true;
            _isSecondJump = false;
            _anim.SetBool("onGround", true);
        }
    }


    /* Health Handlers */
    public void TakeDamage(int value, Transform other)
    {
        _health += value;
        _anim.SetTrigger("hurt");
        HurtEffect(other);
        _managerUI.UpdateLive(this);

        if (_health <= 0) Die();
    }
    private void HurtEffect(Transform other)
    {
        Vector3 bounceBackDir = _transform.position - other.position;
        _rb.velocity = Vector2.up * _rb.velocity.y; // Reset velocity x
        _rb.AddForce(bounceBackDir * _stat._bounceBackForce, ForceMode2D.Impulse);
    }
    private void Die()
    {
        DeactivatePlayer();
    }


    /* Attack Handlers */
    private void AttackAnimation()
    {
        int randomAttackAnimation = Random.Range(0, 2);

        if (_moveInput == 0)
            _anim.SetTrigger(randomAttackAnimation == 1 ? "attack1" : "attack2");                  
            _anim.SetTrigger(randomAttackAnimation == 1 ? "walkAttack1" : "walkAttack2");   
    }
    private void Attack()
    {
        int facingDir = (int)_transform.localScale.x;
        Vector3 hitPos = _transform.position + Vector3.right * _stat._attackRange / 2 * facingDir;
        Collider2D[] hit = Physics2D.OverlapCircleAll(hitPos, _stat._attackRange/2, LayerMask.GetMask("Duck"));

        if (hit.Length != 0)
            foreach (Collider2D enemy in hit)
                if(enemy != _collider)
                    enemy.GetComponent<Enemy_Behavior>().TakeDamage(-1, _transform);
    }


    /* Player_Manger helper methods */
    public void ReactivatePlayer()
    {
        _shieldSprite.SetActive(true);
        _moveInput = 0;
        _isOnGround = false;
        this.gameObject.layer = LayerMask.NameToLayer("Duck");
        this.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(TriggerCanDoThings());
    }
    private void DeactivatePlayer()
    {
        _isCanDoThings = false;
        _health--;
        _managerUI.UpdateLive(this);
        _manager.PlayerDieEvent(this);
        this.gameObject.SetActive(false);
    }
    private IEnumerator TriggerCanDoThings()
    {
        yield return new WaitForSeconds(3f);
        _shieldSprite.SetActive(false);
        _isCanDoThings = true;
        this.gameObject.layer = LayerMask.NameToLayer("Player");
    }



    /* Input Handlers => New Input System */
    private void OnMove(InputValue value)  
    {
        _moveInput = value.Get<Vector2>().x;
        _anim.SetInteger("moveInput", (int)_moveInput);
        if (_moveInput != 0) _transform.localScale = new Vector3((int)_moveInput, 1, 1);    // Flip player to direction
    }
    private void OnJump()
    {
        Jump();
    }
    private void OnDescend()
    {
        Descend();
    }
    private void OnAttack()
    {
        if (_attackCoolDown >= _stat._attackCooldown && _isCanDoThings)
        {
            _attackCoolDown = 0;
            Attack();   
            AttackAnimation();
        }
    }
}
