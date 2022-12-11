using UnityEngine;

public class EnableAttack : MonoBehaviour
{
    public FightingSystem[] Enemies;
    
    public void OnEnableAttack()
    {
        foreach (var enemy in Enemies)
            enemy.CanAttack = true;
    }

    public void OnDisableAttack()
    {
        foreach (var enemy in Enemies)
            enemy.CanAttack = false;
    }
}
