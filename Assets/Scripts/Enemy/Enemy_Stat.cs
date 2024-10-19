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
    [SerializeField] private float jumpForce;
    public float _jumpForce { get => jumpForce; private set { jumpForce = value; } }
}
