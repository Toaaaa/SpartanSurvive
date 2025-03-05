using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _hp = 100;// ����ȭ �ʵ�� ������ �ν����Ϳ��� ���� ������ �� �ְ� ��.
    public float hp
    {
        get => _hp;
        private set => _hp = Mathf.Clamp(value, 0, maxHp); // ü���� ����ġ�� maxHp�� ����.
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
