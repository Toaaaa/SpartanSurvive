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
    [SerializeField] private List<GameObject> weaponInHand;// 손에 배치된 무기

    public event Action onTakeDamage;// 데미지를 받았을 때 이벤트 발생.
    public event Action<int> onStartFX;// 버프 이펙트 시작 이벤트 발생.

    private ThirdPersonController _thirdPersonController;
    private CharacterController _characterController;
    private Animator _animator;
    public bool isAttacking = false;
    public int attackCount = 0;// 최초피격 공격판정 및 횟수 조절 변수.
    public bool isClimbing = false;

    private void Awake()
    {
        _thirdPersonController = GetComponent<ThirdPersonController>();
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        GetComponent<PlayerInput>().enabled = !isDead; // 죽으면 플레이어 입력 비활성화.
        _animator.SetBool("Dead", isDead);// 죽었는지 여부에 따라 애니메이션 변경.
        if(Input.GetKeyDown(KeyCode.G) && _weapon != null) UnEquipWeapon(); // 장착중인 무기 해제.
        _thirdPersonController._DoubleJumpPossible = isDoubleJump;// 더블 점프 버프 적용 여부에 따라 더블 점프 가능 여부 변경.
        _animator.SetBool("HasWeapon", !_weapon.IsUnityNull());// 무기 장착 여부에 따라 애니메이션 변경.
    }
    private void FixedUpdate()
    {
        if (platformTransform != null)
        {
            // 플랫폼의 이동량을 계산하여 플레이어 위치 업데이트
            Vector3 platformMovement = platformTransform.position - lastPlatformPosition;
            transform.position += platformMovement;

            // 다음 프레임을 위해 플랫폼 위치 저장
            lastPlatformPosition = platformTransform.position;
        }
    }

    // 수치 조정
    public void TakeDamage(float damage)// damage 수치 만큼 체력 감소.
    {
        if (isDefence||IsDead) return;// 방어 버프가 활성화 or 사망 되어있으면 데미지를 받지 않음.
        hp -= damage;
        onTakeDamage?.Invoke();// 데미지를 받았을 때 이벤트 발생.
        isDead = hp <= 0;// 체력이 0 이하면 죽음.
    }
    public float GetDamage()// 공격력 반환
    {
        return attackPower;
    }
    public void Heal(float heal)// heal 수치 만큼 체력 회복
    {
        hp += heal;
        onStartFX(0);// 체력 회복 이펙트 시작
        if(hp >= maxHp) hp = maxHp;
    }
    public void DoubleJumpBuffOn()// 더블 점프 버프 적용
    {
        isDoubleJump = true;
        onStartFX(2);// 더블 점프 버프 이펙트 시작
        StartCoroutine(DoubleJumpBuffOff(_doubleJumpBuffTime));
    }
    public void DefenceBuffOn()// 방어 버프 적용
    {
        isDefence = true;
        onStartFX(1);// 방어 버프 이펙트 시작
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
            EquipInHand(e);
        }
        else
        {
            _weapon = w;// 무기 장착
            EquipmentItme e = w as EquipmentItme;
            attackPower += e.itemValue;// 공격력 증가
            EquipInHand(e);
        }

        //Destroy(w.gameObject);// destroy는 레이캐스팅에서 상호작용시 처리하도록 할것.

    }
    public void UnEquipWeapon()
    {
        Instantiate(_weapon.prefab, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity);// 무기 프리팹 생성
        EquipmentItme e = _weapon as EquipmentItme;
        attackPower -= e.itemValue;// 공격력 감소
        _weapon = null;
        for(int i = 0; i < weaponInHand.Count; i++)
        {
            weaponInHand[i].SetActive(false);
        }
    }
    private void EquipInHand(EquipmentItme e)
    {
        foreach(GameObject g in weaponInHand)
        {
            if(g.name == e.itemName)
            {
                g.SetActive(true);
                return;
            }
        }
    }

    // 플렛폼 이동 관련
    private Transform platformTransform;
    private Vector3 lastPlatformPosition;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Physics")) // 물리 판정 오브젝트 일 경우 충돌 전 trigger로 미리 선 처리 하기.
        {
            StartCoroutine(PhysicsControll());
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("LaunchObject"))
        {
            Vector3 collisionDirection = collision.gameObject.GetComponent<PhysicsObjects>().dir; // 충돌 방향 가져오기.
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(collisionDirection * 10, ForceMode.Impulse);
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            platformTransform = collision.transform;
            lastPlatformPosition = platformTransform.position;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            platformTransform = null;
        }
    }

    // 코루틴
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
    IEnumerator PhysicsControll()
    {
        CharacterConController ccc = GetComponent<CharacterConController>();
        Rigidbody rb = GetComponent<Rigidbody>();
        ccc.isLaunch = true;
        _characterController.enabled = false;
        yield return new WaitForSeconds(1.5f);
        ccc.isLaunch = false;
        _characterController.enabled = true;
    }

}
