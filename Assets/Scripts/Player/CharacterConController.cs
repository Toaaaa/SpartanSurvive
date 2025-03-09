using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterConController : MonoBehaviour
{
    CharacterController cc;
    Coroutine coroutine;
    public bool isLaunch = false; // 물리 판정 오브젝트와 충돌하였을때.

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Physics") // 물리 판정이 있는 오브젝트와 충돌시
        {
            cc.enabled = false; // 캐릭터 컨트롤러 비활성화
        }
        if(collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Platform") // 땅/플랫폼 과 충돌시 즉시 cc활성화 
        {
            if (isLaunch) return; // 물리 판정 오브젝트와 충돌하였을때는 리턴
            foreach(ContactPoint contact in collision.contacts) // 충돌한 지점을 모두 가져옴
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f) // 충돌한 지점의 법선벡터가 위쪽이라면 
                {
                    if (coroutine != null) StopCoroutine(coroutine); // 코루틴이 실행중이라면 중지
                    cc.enabled = true; // 캐릭터 컨트롤러 활성화
                    return;
                }
            }
            // 발이 아닌 부분과 충돌했을때 cc를 활성화 하지 않도록 함.
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Physics" || collision.gameObject.tag == "LaunchObject") // 물리 판정이 있는 오브젝트와 충돌중일때
        {
            cc.enabled = false; // 캐릭터 컨트롤러 비활성화
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Physics" ) // 물리 판정이 있는 오브젝트와 충돌이 끝났을때
        {
            PhysicsObject po = collision.gameObject.GetComponent<PhysicsObject>(); // 충돌한 오브젝트의 PhysicsObject 컴포넌트를 가져옴

            if (po == null) return; // 충돌한 오브젝트가 PhysicsObject 컴포넌트를 가지고 있지 않다면 리턴
            coroutine = StartCoroutine(WaitForTime(po.physicTimeScale)); // timeScale 뒤에 캐릭터 컨트롤러 활성화
        }
    }

    IEnumerator WaitForTime(float time)
    {
        yield return new WaitForSeconds(time);
        cc.enabled = true; // 캐릭터 컨트롤러 활성화
    }
}
