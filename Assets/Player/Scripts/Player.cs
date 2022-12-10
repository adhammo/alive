using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Locomotion))]
[RequireComponent(typeof(Fighter))]
[RequireComponent(typeof(Death))]
public class Player : MonoBehaviour
{
    private CharacterController _controller;
    private Locomotion _locomotion;
    private Fighter _fighter;
    private Death _death;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _locomotion = GetComponent<Locomotion>();
        _fighter = GetComponent<Fighter>();
        _death = GetComponent<Death>();
    }

    public void EnablePlayer()
    {
        _controller.enabled = true;
        _locomotion.enabled = true;
        _fighter.enabled = true;
        _death.enabled = true;
    }

    public void DisablePlayer()
    {
        _controller.enabled = false;
        _locomotion.enabled = false;
        _fighter.enabled = false;
        _death.enabled = false;
    }

    public void SetAttacking(bool canAttack)
    {
        if (_fighter.CanAttack != canAttack)
            _fighter.SwordGameObject.SetActive(canAttack);
        _fighter.CanAttack = canAttack;
    }
}
