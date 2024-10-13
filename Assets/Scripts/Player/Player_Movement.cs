using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

[RequireComponent(typeof(PlayerInput))]
public class Player_Movement : MonoBehaviour
{
    [SerializeField] private Player_Stat _stat;
    [SerializeField] private Animator _anim;


    public float _moveInput { get; private set; }
    private Rigidbody2D _rb;
    private bool _isOnGround;
    private Transform _transform;
    private float groundCheckDistance = 1f;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _transform = transform;
    }
    private void FixedUpdate()
    {
        Move();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GroundCheck();
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(_transform.position, Vector2.down * groundCheckDistance);
        if (hit.collider != null)
        {
            _isOnGround = true;
            _anim.SetBool("onGround", true);
        }
    }
    private void Move()
    {
        _rb.velocity += Vector2.right * _moveInput * _stat._moveSpeed * Time.deltaTime;        
    }
    private void Jump()
    {
        if (_isOnGround)
        {
            _rb.AddForce(Vector2.up * _stat._jumpForce, ForceMode2D.Impulse);
            _anim.SetTrigger("jump");
            _anim.SetBool("onGround", false);
            _isOnGround = false;
        }
    }

    private void OnMove(InputValue value)   // INPUT HANDLERS
    {
        _moveInput = value.Get<Vector2>().x;
        _anim.SetInteger("moveInput", (int)_moveInput);
        if (_moveInput != 0) _transform.localScale = new Vector3((int)_moveInput, 1, 1);
    }
    private void OnJump()
    {
        Jump();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector2.down * groundCheckDistance);
    }
}
