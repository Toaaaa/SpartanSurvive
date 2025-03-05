using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _hp = 100;// 직렬화 필드로 선언해 인스펙터에서 값을 변경할 수 있게 함.
    public float hp
    {
        get => _hp;
        private set => _hp = Mathf.Clamp(value, 0, maxHp); // 체력의 상한치를 maxHp로 제한.
    }
    private float maxHp = 100;

    public void TakeDamage(float damage)
    {
        hp -= damage;
    }
    public void Heal(float heal)
    {
        hp += heal;
    }
    public float GetMaxHp()
    {
        return maxHp;
    }
}
