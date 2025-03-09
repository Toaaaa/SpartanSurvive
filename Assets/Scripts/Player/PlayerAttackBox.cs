using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttackBox : MonoBehaviour
{
    public Player player;
    BoxCollider boxCollider;// 공격 판정 박스의 콜라이더.
    private void Awake()
    {
        player = GameManager.Instance.player;
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        if(player.isAttacking)
        {
            boxCollider.enabled = true;
        }
        else
        {
            boxCollider.enabled = false;
        }
    }
}
