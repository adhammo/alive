using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBlockMachine : StateMachineBehaviour
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
