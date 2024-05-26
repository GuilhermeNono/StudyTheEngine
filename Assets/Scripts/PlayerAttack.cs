using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] fireBalls;
    
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private float _cooldownTimer = Mathf.Infinity;

    private static readonly int AttackTrigger = Animator.StringToHash("Attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) && _cooldownTimer > attackCooldown && _playerMovement.CanAttack)
            // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
            Attack();

        _cooldownTimer += Time.deltaTime;
    }
    
    private void Attack()
    {
        _animator.SetTrigger(AttackTrigger);
        _cooldownTimer = 0;
        
        //pool fireballs
        fireBalls[AvailableFireballIndex()].transform.position = firePoint.position;
        // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
        fireBalls[AvailableFireballIndex()].GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private int AvailableFireballIndex()
    {
        for (int i = 0; i < fireBalls.Length; i++)
        {
            if (!fireBalls[i].activeInHierarchy)
                return i;
        }

        return 0;
    }
}