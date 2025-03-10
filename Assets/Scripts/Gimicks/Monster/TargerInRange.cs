using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargerInRange : MonoBehaviour
{
    public bool isInRange;
    public float range = 15f;

    private void Awake()
    {
        GetComponentInParent<Monster>().InRange += GetIsInRange;// 몬스터의 InRange func에 GetIsInRange를 추가.
        GetComponent<SphereCollider>().radius = range;// 스피어 콜라이더의 반지름을 range로 설정.
    }

    public bool GetIsInRange()
    {
        return isInRange;
    }

    // trigger 로 플레이어 범위내 판정
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}
