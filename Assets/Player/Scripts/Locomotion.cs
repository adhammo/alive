using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class Locomotion : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 4.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    public float SprintSpeed = 6.0f;
    [Tooltip("How fast the character turns to face movement direction")]
    public float RotationSmoothTime = 0.12f;
    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;
    [Tooltip("Change speed based on input sensitivity")]
    public bool AnalogMovement = false;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float JumpHeight = 1.2f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -9.81f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float JumpTimeout = 0.1f;
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;

    [Header("Grounded")]
    [Tooltip("Useful for rough ground")]
    public float GroundedOffset = -0.14f;
    [Tooltip("When falling ground offset factor")]
    public float GroundedOffsetFall = 0.1f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.5f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    [Header("Camera")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    public float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    public float BottomClamp = -90.0f;
    [Tooltip("Rotation speed of the camera")]
    public float RotationSpeed = 1.0f;

    [Header("Sounds")]
    [Tooltip("Voice audio source")]
    public AudioSource VoiceSource;
    [Tooltip("Jumping audio clip")]
    public AudioClip JumpingAudioClip;
    [Tooltip("Landing audio clip")]
    public AudioClip LandingAudioClip;
    [Tooltip("Footsteps audio clips")]
    public AudioClip[] FootstepAudioClips;
    [Tooltip("Audio effects volume")]
    [Range(0, 1)] public float AudioVolume = 0.5f;

    [HideInInspector()]
    public bool CanJump = true;
    [HideInInspector()]
    public bool CanLocomote = true;
    
    public bool IsGrounded { get { return _grounded && !_jumped; } }

    // input
    private Vector2 _move;
    private Vector2 _look;
    private bool _jump;
    private bool _sprint;

    // cinemachine
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    // player
    private float _speed;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _stickingVelocity = -4.0f;

    // grounded
    [Header("Stats")]
    [Tooltip("If the character is grounded or not")]
    [SerializeField]
    private bool _grounded;
    private bool _jumped;
    private bool _fallMove;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animator hashes
    private int _moveAnimHash = Animator.StringToHash("Move");
    private int _runAnimHash = Animator.StringToHash("Run");
    private int _fallAnimHash = Animator.StringToHash("Fall");
    private int _jumpAnimHash = Animator.StringToHash("Jump");
    private int _groundAnimHash = Animator.StringToHash("Ground");
    private int _fallMoveAnimHash = Animator.StringToHash("FallMove");

    private GameObject _mainCamera;
    private PlayerInput _playerInput;
    private CharacterController _controller;
    private Animator _anim;

    private const float _threshold = 0.01f;

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;

        _controller = GetComponent<CharacterController>();
        _playerInput = GetComponent<PlayerInput>();
        _anim = GetComponent<Animator>();

        _grounded = false;
        _jumped = false;
        _fallMove = false;

        // reset our timeouts on start
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
    }

    private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    public void OnMove(InputValue value)
    {
        _move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        _look = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        _jump = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        _sprint = value.isPressed;
    }

    private void Update()
    {
        JumpAndGravity();
        Movement();
    }

    private void FixedUpdate()
    {
        GroundedCheck();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = transform.position + Vector3.down * GroundedOffset;
        Vector3 fallSpherePosition = transform.position + Vector3.down * GroundedOffsetFall;
        bool grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        bool fallGrounded = Physics.CheckSphere(fallSpherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        if (!grounded && _grounded) _fallMove = (_speed > 0.0f) && _sprint;
        _grounded = grounded;

        // set animator ground
        _anim.SetBool(_groundAnimHash, fallGrounded || grounded);
        _anim.SetFloat(_fallMoveAnimHash, _fallMove ? 1f : 0f);
    }

    private void JumpAndGravity()
    {
        // apply gravity over time
        _verticalVelocity += Gravity * Time.deltaTime;

        if (_grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = FallTimeout;

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = _stickingVelocity;
            }

            // jump timeout
            if (_jumpTimeoutDelta > 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
            else if (CanJump && !_jumped && _jump)
            {
                // mark jumped
                _jumped = true;

                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                // set animator jump
                _anim.SetTrigger(_jumpAnimHash);
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumped = false;
            _jumpTimeoutDelta = JumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta > 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
        }

        // set animator fall
        _anim.SetBool(_fallAnimHash, _fallTimeoutDelta <= 0);
    }

    private void Movement()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _sprint ? SprintSpeed : MoveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (!CanLocomote || _move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = AnalogMovement ? _move.magnitude : 1f;

        // accelerate or decelerate to target speed
        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            // creates curved result rather than a linear one giving a more organic speed change
            // note T in Lerp is clamped, so we don't need to clamp our speed
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, SpeedChangeRate * Time.deltaTime);

            // round speed to 3 decimal places
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        // input direction
        Vector3 input = new Vector3(_move.x, 0.0f, _move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (CanLocomote && _move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

        // move the player
        Vector3 velocity = targetDirection * _speed + Vector3.up * (_verticalVelocity + 0.5f * Gravity * Time.deltaTime);
        _controller.Move(velocity * Time.deltaTime);

        // set animator move and run
        _anim.SetBool(_moveAnimHash, _move != Vector2.zero);
        _anim.SetBool(_runAnimHash, _sprint);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void CameraRotation()
    {
        // if there is an input and camera position is not fixed
        if (_look.sqrMagnitude >= _threshold)
        {
            // don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _look.x * RotationSpeed * deltaTimeMultiplier;
            _cinemachineTargetPitch += _look.y * RotationSpeed * deltaTimeMultiplier;
        }

        // clamp our rotation
        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

        // update Cinemachine camera target
        CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (_grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;

        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Vector3 spherePosition = transform.position + Vector3.down * GroundedOffset;
        Gizmos.DrawSphere(spherePosition, GroundedRadius);
    }

    private void OnFootstepClip(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (FootstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, FootstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.position, AudioVolume);
            }
        }
    }

    private void OnJumpClip(AnimationEvent animationEvent)
    {
        VoiceSource.PlayOneShot(JumpingAudioClip);
    }

    private void OnLandClip(AnimationEvent animationEvent)
    {
        AudioSource.PlayClipAtPoint(LandingAudioClip, transform.position, AudioVolume);
    }
}