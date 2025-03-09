using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Monster Status")]
    [SerializeField] private bool isDead = false;
    [SerializeField] private float maxHp = 20;
    [SerializeField] private float _hp;
    [Header("HP Bar")]
    [SerializeField] private RectTransform hpBar;// 체력이 최대일때는 Width가 2.


    public float hp
    {
        get => _hp;
        private set => _hp = value;
    }
    [SerializeField] private int _attackPower;
    public int attackPower
    {
        get => _attackPower;
        set => _attackPower = value;
    }

    private void OnEnable()
    {
        isDead = false;
        _hp = maxHp;
    }

    private void Update()
    {
        if (hp <= 0)
        {
            isDead = true;
            gameObject.SetActive(false);
        }
        if(hpBar != null)
        {
            hpBar.sizeDelta = new Vector2(hp/maxHp * 2, hpBar.sizeDelta.y);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackBox"))// 공격의 판정박스에 닿았을 때.
        {
            Player player = other.GetComponent<PlayerAttackBox>().player;
            if (player == null || player.attackCount <= 0) return; // 플레이어의 공격이 아니거나 공격 판정 잔여 횟수가 0일때는 return.
            hp -= player.GetDamage();
            player.attackCount--;
           
        }
    }
}
