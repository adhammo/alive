using UnityEngine;

public class PlayerBlockMachine : StateMachineBehaviour
{
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Locomotion locomotion = animator.GetComponent<Locomotion>();
        locomotion.CanLocomote = false;
        Death death = animator.GetComponent<Death>();
        death.CanHurt = true;
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        Locomotion locomotion = animator.GetComponent<Locomotion>();
        locomotion.CanLocomote = true;
        Death death = animator.GetComponent<Death>();
        death.CanHurt = false;
    }
}
