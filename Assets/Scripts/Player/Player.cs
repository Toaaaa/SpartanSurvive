using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private bool isDead = false;// 죽었는지 여부
    public bool IsDead
    {
        get => isDead;
        set => isDead = value;
    }
    //체력
    private float maxHp = 100;// 최대 체력 100
    [SerializeField] private float _hp = 100;// 직렬화 필드로 선언해 인스펙터에서 값을 변경할 수 있게 함.
    public float hp
    {
        get => _hp;
        private set => _hp = Mathf.Clamp(value, 0, maxHp); // 체력의 상한치를 maxHp로 제한.
    }
    //더블 점프 버프
    [SerializeField] private bool _isDoubleJump = false;// 더블 점프 버프 적용 여부.
    [SerializeField] private float _doubleJumpBuffTime = 5f;// 더블 점프 버프 지속 시간.
    public bool isDoubleJump
    {
        get => _isDoubleJump;
        set => _isDoubleJump = value;
    }
    //방어 버프
    [SerializeField] private bool _isDefence = false;// 방어 버프 적용 여부.
    [SerializeField] private float _defenceBuffTime = 5f;// 방어 버프 지속 시간.
    public bool isDefence
    {
        get => _isDefence;
        set => _isDefence = value;
    }



    public void TakeDamage(float damage)// damage 수치 만큼 체력 감소.
    {
        hp -= damage;
        isDead = hp <= 0;// 체력이 0 이하면 죽음.
    }
    public void Heal(float heal)// heal 수치 만큼 체력 회복
    {
        hp += heal;
        if(hp >= maxHp) hp = maxHp;
    }
    public void DoubleJumpBuffOn()// 더블 점프 버프 적용
    {
        isDoubleJump = true;
        StartCoroutine(DoubleJumpBuffOff(_doubleJumpBuffTime));
    }
    public void DefenceBuffOn()// 방어 버프 적용
    {
        isDefence = true;
        StartCoroutine(DefenceBuffOff(_defenceBuffTime));
    }
    public float GetMaxHp()
    {
        return maxHp;
    }
    public float GetDoubleJumpBuffTime()
    {
        return _doubleJumpBuffTime;
    }
    public float GetDefenceBuffTime()
    {
        return _defenceBuffTime;
    }

    IEnumerator DoubleJumpBuffOff(float time)// 더블 점프 버프 해제
    {
        yield return new WaitForSeconds(time);
        isDoubleJump = false;
    }
    IEnumerator DefenceBuffOff(float time)// 방어 버프 해제
    {
        yield return new WaitForSeconds(time);
        isDefence = false;
    }

}
