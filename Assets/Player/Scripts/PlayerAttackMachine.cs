using UnityEngine;

public class PlayerAttackMachine : StateMachineBehaviour
{
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        animator.applyRootMotion = true;
        Locomotion locomotion = animator.GetComponent<Locomotion>();
        locomotion.CanLocomote = false;
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.applyRootMotion = false;
        Locomotion locomotion = animator.GetComponent<Locomotion>();
        locomotion.CanLocomote = true;
    }
}
