using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuffController : MonoBehaviour
{
    Player player;
    public GameObject doubleJumpBuffIcon;
    float doubleJumpCurrentTime;
    float doubleJumpBuffTime;
    public GameObject defenceBuffIcon;
    float defenceCurrentTime;
    float defenceBuffTime;

    private void Awake()
    {
        player = GameManager.Instance.player;
        doubleJumpBuffTime = player.GetDoubleJumpBuffTime();
        defenceBuffTime = player.GetDefenceBuffTime();
    }

    private void Update()
    {
        doubleJumpBuffIcon.SetActive(player.isDoubleJump);
        defenceBuffIcon.SetActive(player.isDefence);

        if(player.isDoubleJump)
        {
            Image timer = doubleJumpBuffIcon.transform.GetChild(0).GetComponent<Image>();
            doubleJumpCurrentTime -= Time.deltaTime;
            timer.fillAmount = doubleJumpCurrentTime / doubleJumpBuffTime;// 버프 지속 시간에 따른 타이머 갱신.
        }
        else { doubleJumpCurrentTime = doubleJumpBuffTime; }// 버프가 적용되지 않았을 때 타이머 초기화.

        if(player.isDefence)
        {
            Image timer = defenceBuffIcon.transform.GetChild(0).GetComponent<Image>();
            defenceCurrentTime -= Time.deltaTime;
            timer.fillAmount = defenceCurrentTime / defenceBuffTime;// 버프 지속 시간에 따른 타이머 갱신.
        }
        else { defenceCurrentTime = defenceBuffTime; }// 버프가 적용되지 않았을 때 타이머 초기화.
    }
}
