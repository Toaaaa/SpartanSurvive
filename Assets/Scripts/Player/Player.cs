using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamageable
{
        [Header("Player Status")]
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
    //공격력
    [SerializeField] private float _attackPower = 1;// 공격력
    public float attackPower
    {
        get => _attackPower;
        set => _attackPower = value;
    }

        [Header("Player Buffs")]
    //더블 점프 버프
    [SerializeField] private bool _isDoubleJump = false;// 더블 점프 버프 적용 여부.
    [SerializeField] private float _doubleJumpBuffTime = 8f;// 더블 점프 버프 지속 시간.
    public bool isDoubleJump
    {
        get => _isDoubleJump;
        set => _isDoubleJump = value;
    }
    //방어 버프
    [SerializeField] private bool _isDefence = false;// 방어 버프 적용 여부.
    [SerializeField] private float _defenceBuffTime = 10f;// 방어 버프 지속 시간.
    public bool isDefence
    {
        get => _isDefence;
        set => _isDefence = value;
    }

        [Header("EquipItem")]
    [SerializeField] private ItemData _weapon;// 장착한 무기

    public event Action onTakeDamage;// 데미지를 받았을 때 이벤트 발생.

    private ThirdPersonController _thirdPersonController;

    private void Awake()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
    }

    private void Update()
    {
        GetComponent<PlayerInput>().enabled = !isDead; // 죽으면 플레이어 입력 비활성화.
        if(Input.GetKeyDown(KeyCode.G) && _weapon != null) UnEquipWeapon(); // 장착중인 무기 해제.
        _thirdPersonController._DoubleJumpPossible = isDoubleJump;// 더블 점프 버프 적용 여부에 따라 더블 점프 가능 여부 변경.
    }

    // 수치 조정
    public void TakeDamage(float damage)// damage 수치 만큼 체력 감소.
    {
        if (isDefence) return;// 방어 버프가 활성화 되어있으면 데미지를 받지 않음.
        hp -= damage;
        onTakeDamage?.Invoke();// 데미지를 받았을 때 이벤트 발생.
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
    
    // Get
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

    // Set
    public void EquipWeapon(ItemData w)// 무기 장착
    {
        if(_weapon != null)// 이미 무기를 장착 중인 경우
        {
            UnEquipWeapon();
            _weapon = w;// 새로운 무기 장착
            EquipmentItme e = w as EquipmentItme;
            attackPower += e.itemValue;// 공격력 증가
        }
        else
        {
            _weapon = w;// 무기 장착
            EquipmentItme e = w as EquipmentItme;
            attackPower += e.itemValue;// 공격력 증가
        }

        //Destroy(w.gameObject);// destroy는 레이캐스팅에서 상호작용시 처리하도록 할것.

    }
    public void UnEquipWeapon()
    {
        Instantiate(_weapon.prefab, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity);// 무기 프리팹 생성
        EquipmentItme e = _weapon as EquipmentItme;
        attackPower -= e.itemValue;// 공격력 감소
        _weapon = null;
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
