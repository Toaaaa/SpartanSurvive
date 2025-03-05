using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TimeController : MonoBehaviour
{
    public Material skyBoxMaterial;
    [Range(0, 1)] public float timeofDay;
    public bool upTime = true;// timeofDay�� 0����1�� ���� true, 1���� 0���� ���� false.

    // Update is called once per frame
    void Update()
    {
        skyBoxMaterial.SetFloat("_CubemapTransition", timeofDay);// timeofDay�� ���̴��� Cubemap Transition ������ 1��1 ���� ����.
        skyBoxMaterial.SetFloat("_Exposure", Mathf.Lerp(1f,0.5f,timeofDay));// timeofDay(0 ~ 1) �� ���� Cubemap Exposure���� 1���� 0.5�� ���� ����.
        if(upTime)
        {
            if (timeofDay >= 1)
            {
                upTime = false;
            }
            timeofDay += Time.deltaTime * 0.0034f;// 300�ʿ� 12�ð� 0���� 1�� ����.
        }
        else
        {
            if (timeofDay <= 0)
            {
                upTime = true;
            }
            timeofDay -= Time.deltaTime * 0.0034f;// 300�ʿ� 12�ð� 1���� 0���� ����.
        }
    }
}
