using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [SerializeField] private Player_Stat _stat;
    [SerializeField] private Animator _anim;
    private Player_Movement _pMove;
    private float _attackCoolDown;

    private void Awake()
    {
        _pMove = GetComponent<Player_Movement>();
    }

    private void Update()
    {
        _attackCoolDown += Time.deltaTime;
    }

    private void AttackAnimation()
    {
        int randomAttackAnimation = Random.Range(0, 2);

        if (_pMove._moveInput == 0)
            _anim.SetTrigger(randomAttackAnimation == 1 ? "attack1" : "attack2");
        else
            _anim.SetTrigger(randomAttackAnimation == 1 ? "walkAttack1" : "walkAttack2");
    }
    private void OnAttack() // INPUT HANDLER
    {
        if (_attackCoolDown >= _stat._attackCooldown)
        {
            _attackCoolDown = 0;
            AttackAnimation();
        }
    }
}
