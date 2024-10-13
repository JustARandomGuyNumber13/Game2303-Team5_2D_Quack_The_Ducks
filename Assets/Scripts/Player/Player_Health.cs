using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Health : MonoBehaviour
{
    [SerializeField] private Player_Stat _stat;
    [SerializeField] private Animator _anim;

    private int _health;
    private bool isAlive = true;


    public void TakeDamage(int value)
    {
        _health -= value;
        if (_health <= 0) Die();
    }

    private void Die()
    {
        isAlive = false;
        this.gameObject.SetActive(false);
    }
}
