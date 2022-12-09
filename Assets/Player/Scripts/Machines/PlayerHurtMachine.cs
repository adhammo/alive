using UnityEngine;

public class PlayerHurtMachine : StateMachineBehaviour
{
     override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        Locomotion locomotion = animator.GetComponent<Locomotion>();
        locomotion.CanLocomote = false;
    }

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        Locomotion locomotion = animator.GetComponent<Locomotion>();
        locomotion.CanLocomote = true;
    }
}
