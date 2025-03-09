using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]Vector3 initPos;
    [SerializeField]Vector3 targetPos;
    public bool startToLeft = true; // true일때 처음 움직임이 왼쪽으로 움직이기 시작. false일때 오른쪽으로 움직이기 시작.

    private void Awake()
    {
        initPos = transform.position; ;
        if(startToLeft)
            targetPos = transform.position + new Vector3(-3,0,0);
        else
            targetPos = transform.position + new Vector3(3,0,0);
    }

    private void Update()
    {
        if(startToLeft)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                startToLeft = false;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, initPos, Time.deltaTime);
            if (Vector3.Distance(transform.position, initPos) < 0.1f)
            {
                startToLeft = true;
            }
        }
    }
}
