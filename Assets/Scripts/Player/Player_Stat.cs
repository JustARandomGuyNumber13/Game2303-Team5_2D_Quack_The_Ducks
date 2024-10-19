using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStatData", menuName = "Player Stat")]
public class Player_Stat : ScriptableObject
{
    [SerializeField] private int maxHealth;
    public int _maxHealth { get => maxHealth; private set { maxHealth = value; } }

    [SerializeField] private float moveSpeed;
    public float _moveSpeed { get => moveSpeed; private set {moveSpeed = value; } }

    [SerializeField] private float jumpForce;
    public float _jumpForce { get => jumpForce; private set { jumpForce = value; } }

    [SerializeField] private float attackCooldown;
    public float _attackCooldown { get => attackCooldown; private set { attackCooldown = value; } }

    [SerializeField] private float accelerationSpeed;
    public float _accelerationSpeed { get => accelerationSpeed; private set { accelerationSpeed = value; } }
}
