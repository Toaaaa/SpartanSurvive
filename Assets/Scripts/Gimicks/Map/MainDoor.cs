using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class MainDoor : Interactable
{
    [SerializeField] float startAngle; // 문이 닫힌 상태의 최초 각도
    [SerializeField] float endAngle; // 문이 열린 상태의 최종 각도

    private bool isOpen = false;
    private bool isAction = false;// 문이 움직이는 중인지.

    override public void Interact()
    {
        OpenDoor();
    }
    public void OpenDoor()
    {
        if (isAction) return;
        AudioManager.instance.PlaySFX(2);// 문 사운드 재생
        if (isOpen)
        {
            StartCoroutine(MoveDoor(startAngle));// 문을 닫는다.(-90 도)
        }
        else
        {
            StartCoroutine(MoveDoor(endAngle));// 문을 연다.(-180 도)
        }
    }
    IEnumerator MoveDoor(float angle)
    {
        isAction = true;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, angle, 0);
        float time = 0;
        float duration = 1.5f;
        while (time < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time / duration);// time/duration은 진척도라고 보면 됨.(time이 duration만큼 진행되면 1이 됨)
            time += Time.deltaTime;
            yield return null;
        }
        transform.rotation = endRotation;
        isOpen = !isOpen;
        isAction = false;
    }
}
