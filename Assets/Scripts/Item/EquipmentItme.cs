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
        player.EquipWeapon(this);
    }
}
