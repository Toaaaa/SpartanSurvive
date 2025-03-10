using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    armor,
}

[CreateAssetMenu(fileName = "New Equipment Item", menuName = "Items/Equipment")]
public class EquipmentItme : ItemData
{
    public EquipmentType equipmentType;
    public int itemValue;// 공격력 수치 or 방어력 수치

    public override void Use(Player player)
    {
        AudioManager.instance.PlaySFX(1);// 장비 상호작용 사운드 재생
        player.EquipWeapon(this);
    }
}
