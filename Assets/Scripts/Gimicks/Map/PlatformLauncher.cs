using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLauncher : Interactable
{
    public GameObject launcher;// 발사 판정 부분. (평소에는 비활성화 콜라이더 오브젝트)

    override public void Interact()
    {
        Debug.Log("Interact");
        StartCoroutine(Launch());
    }
    IEnumerator Launch()
    {
        launcher.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        launcher.SetActive(false);
    }
}
