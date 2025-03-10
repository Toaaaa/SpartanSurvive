using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;// 해당 아이템 게임 오브젝트의 아이템 데이터.
    public float yoyoY;

    private void Start()
    {
        UpdownMove();
        yoyoY = transform.position.y + 0.2f;
    }

    void UpdownMove()
    {
        transform.DOLocalMoveY(yoyoY, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
