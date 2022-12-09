using UnityEngine;

public class PlayerLocomotionMachine : StateMachineBehaviour
{
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Death death = animator.GetComponent<Death>();
        death.CanHurt = true;
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        Death death = animator.GetComponent<Death>();
        death.CanHurt = false;
    }
}
