using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.10f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Custom Variable")]
        public bool _DoubleJumpPossible;
        public int _JumpCount = 0;
        Player player;
        [SerializeField] GameObject playerRay;
        public bool duringClimbEnd = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAttack;
        private int _animClimb;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private LadderClimb _ladderClimb;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            player = GetComponent<Player>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
            _ladderClimb = GetComponent<LadderClimb>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (!player.IsDead)// 죽었을때는 행동을 하지 못함.
            {
                PlayerClimbRaycast();
                JumpAndGravity();
                GroundedCheck();
                Move();
                Attack();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAttack = Animator.StringToHash("Attack");
            _animClimb = Animator.StringToHash("Climb");
        }

        private void GroundedCheck()
        {
            if(player.isClimbing)// 사다리를 타고 있으면 땅에 있음.
            {
                Grounded = false;
                _animator.SetBool(_animIDGrounded, Grounded);
                return;
            }
            _animator.speed = 1;// 사다리를 타는중이 아닐땐 애니메이션 속도 1로 고정.
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            if (player.isAttacking || duringClimbEnd) return;// 공격중 or 오르기를 마무리 하는 중이면 움직이지 않음.
            if (player.isClimbing)
            {
                ClimbMove();
                return;// 사다리를 오르는 중이면 다른 움직임 Input 받지 않음.
            }

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                if (_fallTimeoutDelta <= 0.0f)
                {
                    _JumpCount = 0;
                }
                _fallTimeoutDelta = FallTimeout;
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump )
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    Grounded = false;
                    _JumpCount = 1;
                    _input.jump = false;
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                if(_input.jump&&!_DoubleJumpPossible&&!player.isClimbing)
                {
                    _input.jump = false;
                }

                if (_DoubleJumpPossible && _input.jump && _JumpCount == 1)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                    _JumpCount++;
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                    _input.jump = false;
                }
                if(_DoubleJumpPossible && _input.jump && _JumpCount >= 2)
                    _input.jump = false;
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator&&!player.isClimbing)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump                
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        private void Attack()
        {
            if (_input.attack&&Grounded)
            {
                if (_hasAnimator)
                {
                    _animator.SetTrigger(_animIDAttack);
                    player.isAttacking = true;
                    _input.attack = false;
                }
            }
            if(_input.attack &&!Grounded)
            {
                _input.attack = false;
            }
        }
        private void Climb(Vector3 hitpoint)// 사다리 오르는 상태로 전환
        {            
            if(player.isClimbing)// 타고있는 중
            {
                _animator.SetBool(_animClimb, true);
                //타고있는 도중에는 항상 z축 거리를 일정하게(레이캐스팅 거리인 0.5보다 낮게) 유지.
                Vector3 playerPos = playerRay.transform.position;
                Vector3 targetPos = new Vector3(hitpoint.x, playerPos.y, hitpoint.z); // 높이는 유지한 채 XZ만 적용

                float distance = Vector3.Distance(new Vector3(playerPos.x, 0, playerPos.z), new Vector3(hitpoint.x, 0, hitpoint.z));
                // 거리가 일정거리 이상이면 이동 보정
                if (distance > 0.3f)
                {
                    transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 5f);
                }
                if (_input.climb)// 타는 도중 탈출을 누르면 상태 해제.
                {
                    _animator.SetBool(_animClimb, false);
                    player.isClimbing = false;
                    _input.jump = false;
                    _input.climb = false;
                }
            }

            if (_hasAnimator && _input.climb)
            {
                _ladderClimb.LookAtLadder();// 사다리를 바라보게함.
                _animator.SetBool(_animClimb, true);
                player.isClimbing = true;
                _input.climb = false;
            }
        }
        private void ClimbMove()
        {
            float climbSpeed = 2.0f; // 원하는 속도로 변경 가능
            float horizontalClimbSpeed = 1.5f; // 원하는 속도로 변경 가능

            // 위로 이동하는 입력 처리
            float climbVirDirection = _input.move.y; // ↑ : 1, ↓ : -1
            float climbHorDirection = _input.move.x; // → : 1, ← : -1

            if(climbVirDirection == 0 && climbHorDirection == 0)
            {
                _animator.speed = 0;
            }
            else
            {
                _animator.speed = 1;
            }

            // 수직 이동 벡터
            Vector3 verticalMove = Vector3.up * climbVirDirection * climbSpeed * Time.deltaTime;
            // 현재 사다리의 오른쪽 방향 계산
            Vector3 ladderRight = _ladderClimb.transform.right; // 사다리의 오른쪽 방향 벡터

            // 좌우 이동 벡터 계산 (사다리의 오른쪽 방향 기준)
            Vector3 horizontalMove = ladderRight * climbHorDirection * horizontalClimbSpeed * Time.deltaTime;

             Vector3 moveDirection = verticalMove + horizontalMove;
            _controller.Move(moveDirection);

            // 애니메이션 적용
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, Mathf.Abs(climbVirDirection)); // 위/아래 움직임 반영
            }
        }

        void PlayerClimbRaycast()
        {
            Ray ray = new Ray(playerRay.transform.position, playerRay.transform.forward);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 0.2f))
            {
                if (hit.collider.CompareTag("Climbable"))
                {
                    _ladderClimb.SetLadderRotation(hit.collider.gameObject);
                    Climb(hit.point);
                }
                else
                {
                    _input.climb = false;
                    _animator.SetBool(_animClimb, false);
                }
            }
        }
        void OnTriggerEnter(Collider c)
        {
            if(c.gameObject.CompareTag("ClimbEnd"))
            {
                _input.climb = false;
                player.isClimbing = false;
                _animator.SetBool(_animClimb, false);
            }
        }

        IEnumerator SmoothClimbEndMove()
        {
            float duration = 0.8f; // 이동하는 시간
            float elapsedTime = 0f;

            Vector3 startPosition = transform.position;
            Vector3 targetPosition = startPosition + Vector3.forward * 1f + Vector3.up * 1f;

            while (elapsedTime < duration)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 최종 위치 보정
            transform.position = targetPosition;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}