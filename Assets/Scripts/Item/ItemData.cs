using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public int itemID;
    public string itemName;
    public string description;// 마우스를 올렸을때 UI에 표시될 사용시의 설명.
    public GameObject prefab;// 아이템 프리팹 데이터.

    public virtual void Use(Player player)// 상호작용으로 사용.
    {
    }
    public string GetInfo()
    {
        return itemName + "\n" + description;
    }
}
