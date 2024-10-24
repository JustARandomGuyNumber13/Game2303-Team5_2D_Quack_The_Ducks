using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyStatData", menuName = "Enemy Stat")]
public class Enemy_Stat : ScriptableObject
{
    [SerializeField] private int maxHealth;
    public int _maxHealth { get => maxHealth; private set { maxHealth = value; } }

    [SerializeField] private float moveSpeed;
    public float _moveSpeed { get => moveSpeed; private set { moveSpeed = value; } }

    [SerializeField] float moveSpeedIncreaseBy;
    public float _moveSpeedIncreaseBy { get => moveSpeedIncreaseBy; private set { moveSpeedIncreaseBy = value; } }

    [SerializeField] private float jumpForce;
    public float _jumpForce { get => jumpForce; private set { jumpForce = value; } }

    [SerializeField] private float bounceBackForce;
    public float _bounceBackForce { get => bounceBackForce; private set { bounceBackForce = value; } }

    [SerializeField] private float groundCheckDistance;
    public float _groundCheckDistance { get => groundCheckDistance; private set { groundCheckDistance = value; } }

    [SerializeField] private float jumpNodeHeightRequirement;
    public float _jumpNodeHeightRequirement { get => jumpNodeHeightRequirement; private set { jumpNodeHeightRequirement = value; } }

    [SerializeField] private float explosionRange;
    public float _explosionRange { get => explosionRange; private set { explosionRange = value; } }

}
