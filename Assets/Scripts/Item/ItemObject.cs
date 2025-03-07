using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemObject : MonoBehaviour
{
    public ItemData itemData;// 해당 아이템 게임 오브젝트의 아이템 데이터.

    private void Start()
    {
        UpdownMove();
    }

    void UpdownMove()
    {
        transform.DOLocalMoveY(1.2f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }
}
