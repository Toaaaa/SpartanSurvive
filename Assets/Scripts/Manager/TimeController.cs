using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class TimeController : MonoBehaviour
{
    public Material skyBoxMaterial;
    [Range(0, 1)] public float timeofDay;
    public bool upTime = true;// timeofDay가 0에서1로 갈때 true, 1에서 0으로 갈때 false.

    // Update is called once per frame
    void Update()
    {
        skyBoxMaterial.SetFloat("_CubemapTransition", timeofDay);// timeofDay와 쉐이더의 Cubemap Transition 변수를 1대1 비율 연결.
        skyBoxMaterial.SetFloat("_Exposure", Mathf.Lerp(1f,0.5f,timeofDay));// timeofDay(0 ~ 1) 에 따라 Cubemap Exposure값을 1에서 0.5로 선형 보간.
        if(upTime)
        {
            if (timeofDay >= 1)
            {
                upTime = false;
            }
            timeofDay += Time.deltaTime * 0.0034f;// 300초에 12시간 0에서 1로 증가.
        }
        else
        {
            if (timeofDay <= 0)
            {
                upTime = true;
            }
            timeofDay -= Time.deltaTime * 0.0034f;// 300초에 12시간 1에서 0으로 감소.
        }
    }
}
