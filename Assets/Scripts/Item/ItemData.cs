using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    public string itemName;
    public string description;// 마우스를 올렸을때 UI에 표시될 사용시의 설명.

    public virtual void Use()// 상호작용으로 사용.
    {
    }
}
