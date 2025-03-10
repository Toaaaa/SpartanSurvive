using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Item", menuName = "Items/Consumable")]
public class ConsumableItem : ItemData
{
    public int healAmount;

    public override void Use(Player player)
    {
        AudioManager.instance.PlaySFX(0);// 힐 사운드 재생
        player.Heal(healAmount);
    }
}
