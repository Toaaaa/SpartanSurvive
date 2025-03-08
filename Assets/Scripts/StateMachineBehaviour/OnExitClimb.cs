using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnExitClimb : StateMachineBehaviour // 마지막 오르기에서 끝부분의 올라오는 동작의 state에 쓸 스크립트.
{
    private Vector3 finalPosition;
    private Quaternion finalRotation;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var characterController = animator.GetComponent<CharacterController>();
        var rigidbody = animator.GetComponent<Rigidbody>();

        animator.applyRootMotion = true;

        // **중력 비활성화 (추락 방지)**
        if (characterController != null)
        {
            characterController.enabled = false;
        }
        if (rigidbody != null)
        {
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero; // 공중에서 내려오지 않도록 속도 초기화
        }
        animator.SetBool("ClimbEnd", false);
        animator.GetComponent<Player>().isClimbing = false;
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 현재 애니메이션의 마지막 위치와 회전값 저장
        finalPosition = animator.transform.position;
        finalRotation = animator.transform.rotation;

        // Root Motion 비활성화
        animator.applyRootMotion = false;
        animator.SetBool("ClimbEnd", false);
        animator.GetComponent<ThirdPersonController>().duringClimbEnd = false;


        // **위치 고정**
        animator.transform.position = finalPosition;
        animator.transform.rotation = finalRotation;
    }
}
