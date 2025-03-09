using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnExitAttack : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = true;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = false;
        animator.ResetTrigger("Attack");
        animator.GetComponent<Player>().attackCount = 0;// 공격 판정 초기화.
        animator.GetComponent<Player>().isAttacking = false;
    }
}
