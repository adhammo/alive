using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Locomotion))]
[RequireComponent(typeof(Fighter))]
[RequireComponent(typeof(Death))]
public class Player : MonoBehaviour
{
    private CharacterController _controller;
    private Animator _anim;
    private Locomotion _locomotion;
    private Fighter _fighter;
    private Death _death;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _locomotion = GetComponent<Locomotion>();
        _fighter = GetComponent<Fighter>();
        _death = GetComponent<Death>();
    }

    public void EnablePlayer()
    {
        _controller.enabled = true;
        _locomotion.enabled = true;
        _locomotion.ResetCamera();
        _fighter.enabled = true;
        _death.enabled = true;
        _anim.SetLayerWeight(1, 1f);
    }

    public void DisablePlayer()
    {
        _locomotion.ResetCamera();
        _controller.enabled = false;
        _locomotion.Reset();
        _locomotion.enabled = false;
        _fighter.Reset();
        _fighter.enabled = false;
        _death.Reset();
        _death.enabled = false;
        _anim.SetLayerWeight(1, 0f);
    }

    public void SetAttacking(bool canAttack)
    {
        if (_fighter.CanAttack != canAttack)
            _fighter.SwordGameObject.SetActive(canAttack);
        _fighter.CanAttack = canAttack;
    }
}
