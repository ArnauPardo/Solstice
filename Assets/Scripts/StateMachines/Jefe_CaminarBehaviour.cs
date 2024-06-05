using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jefe_CaminarBehaviour : StateMachineBehaviour
{

    private Boss boss;
    private Rigidbody2D rb;
    [SerializeField] private float moveSpeed;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss = animator.GetComponent<Boss>();
        rb = boss.rb;

        boss.isInvulnerable = false;
        boss.LookAtPlayer();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.LookAtPlayer();
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y) * animator.transform.right.x;
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }
}
