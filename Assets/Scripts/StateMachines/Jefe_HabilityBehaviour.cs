using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jefe_HabilityBehaviour : StateMachineBehaviour
{

    private Boss boss;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        boss.Hability();
    }
}
