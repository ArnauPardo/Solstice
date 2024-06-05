using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jefe_idleBehaviour : StateMachineBehaviour
{

    private Boss boss;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();

        boss.isInvulnerable = true;
    }
}
