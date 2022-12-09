using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Locomotion))]
public class Fighter : MonoBehaviour
{
    [Header("Attack")]
    [Tooltip("How fast the character turns to face attack direction")]
    public float AttackRotationSmoothTime = 0.12f;
    [Tooltip("Attack distance in meters")]
    public float AttackDistance = 2.0f;
    [Tooltip("Attack box width in meters")]
    public float AttackWidth = 3.0f;
    [Tooltip("Attack box height in meters")]
    public float AttackHeight = 3.0f;
    [Tooltip("Layer to attack in")]
    public LayerMask AttackLayer;

    [Header("Block")]
    [Tooltip("How fast the character turns to face block direction")]
    public float BlockRotationSmoothTime = 0.12f;
    [Tooltip("Rotate every angle step")]
    public float BlockRotationAngleStep = 30f;
    [Tooltip("Block box width in meters")]
    public float BlockWidth = 1.0f;
    [Tooltip("Block box height in meters")]
    public float BlockHeight = 1.0f;
    [Tooltip("Layer to block from")]
    public LayerMask BlockLayer;

    [Header("Stats")]
    [Tooltip("Attack damage")]
    public float AttackDamage = 40.0f;
    [Tooltip("Block reduction")]
    public float BlockReduction = 0.2f;

    [Header("Weapons")]
    [Tooltip("Sword")]
    public GameObject SwordGameObject;
    [Tooltip("Shield")]
    public GameObject ShieldGameObject;

    [Header("Sounds")]
    [Tooltip("Voice audio source")]
    public AudioSource VoiceSource;
    [Tooltip("Attack audio clips")]
    public AudioClip[] AttackAudioClips;
    [Tooltip("Attack hit audio clips")]
    public AudioClip[] AttackHitAudioClips;
    [Tooltip("Hit audio clips")]
    public AudioClip[] HitAudioClips;
    [Tooltip("Impact audio clips")]
    public AudioClip[] ImpactAudioClips;
    [Tooltip("Audio effects volume")]
    [Range(0, 1)] public float AudioVolume = 0.5f;

    // player
    private float _rotationVelocity;
    private float _targetRotation = 0.0f;

    // attacking
    private bool _attacked;
    private bool _attacking;

    // blocking
    private bool _blocking;

    // input
    private bool _attack;
    private bool _block;

    // animator hashes
    private int _attackAnimHash = Animator.StringToHash("Attack");
    private int _blockAnimHash = Animator.StringToHash("Block");

    private GameObject _mainCamera;
    private Animator _anim;
    private Locomotion _locomotion;

    public void OnAttack(InputValue value)
    {
        _attack = value.isPressed;
    }

    public void OnBlock(InputValue value)
    {
        _block = value.isPressed;
    }

    private void Awake()
    {
        // get a reference to our main camera
        if (_mainCamera == null)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _locomotion = GetComponent<Locomotion>();

        // reset attack register
        _attacked = false;

        // reset state
        _attacking = false;
        _blocking = false;
    }

    private void Update()
    {
        if (_locomotion.IsGrounded)
        {
            // attack and block
            Attack();
            Block();
        }
        else
        {
            // reset state
            _attacking = false;
            _blocking = false;

            // reset animations
            _anim.ResetTrigger(_attackAnimHash);
            _anim.SetBool(_blockAnimHash, false);
        }
    }

    private void Attack()
    {
        if (_attack)
        {
            // reset attack
            _attack = false;

            // register attack
            _attacked = true;
        }

        if (_attacked && !_attacking)
        {
            // unregister attack
            _attacked = false;

            // mark attack
            _attacking = true;

            // record target
            _targetRotation = _mainCamera.transform.eulerAngles.y;

            // trigger attack
            _anim.SetTrigger(_attackAnimHash);
        }

        if (_attacking)
        {
            // rotate to target
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                AttackRotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }
    }

    private void EndAttack()
    {
        // unmark attack
        _attacking = false;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

    private void Block()
    {
        // set blocking
        if (!_attacking && _block)
        {
            if (!_blocking)
                _targetRotation = _mainCamera.transform.eulerAngles.y;
            _blocking = true;
        }
        else
            _blocking = false;

        if (_blocking)
        {
            // discrete target
            float angDiff = ClampAngle(_mainCamera.transform.eulerAngles.y - _targetRotation, float.MinValue, float.MaxValue);
            if (Mathf.Abs(angDiff) > BlockRotationAngleStep)
                _targetRotation = _mainCamera.transform.eulerAngles.y;

            // rotate to target
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                BlockRotationSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        // set animator blocking
        _anim.SetBool(_blockAnimHash, _blocking);
    }

    public bool IsBlocked(Vector3 direction)
    {
        return true;
    }

    private void OnAttackCallback(AnimationEvent animationEvent)
    {
        Collider[] hits = Physics.OverlapBox(transform.position + Vector3.up * (AttackHeight / 2f) + transform.forward * (AttackDistance / 2f - 0.5f), new Vector3(AttackWidth / 2f, AttackHeight / 2f, (AttackDistance / 2f) + 0.5f), Quaternion.LookRotation(transform.forward), AttackLayer);

        Collider minHit = null;
        float minSqrDist = float.PositiveInfinity;
        foreach (var hit in hits)
        {
            if (hit.tag != "Enemy") continue;

            float distanceSqr = (transform.position - hit.transform.position).sqrMagnitude;
            if (distanceSqr < minSqrDist)
            {
                minSqrDist = distanceSqr;
                minHit = hit;
            }
        }

        if (minHit != null)
        {
            Debug.Log("Attack damage");
            // _audioSource.PlayOneShot(AttackHit);
            // BotStatus bot = minHit.GetComponent<BotStatus>();
            // bot.TakeDamage(AttackDamage);
        }
    }

    private void OnAttackClip(AnimationEvent animationEvent)
    {
        if (AttackAudioClips.Length > 0)
        {
            var index = Random.Range(0, AttackAudioClips.Length);
            VoiceSource.PlayOneShot(AttackAudioClips[index], AudioVolume);
        }
    }

    private void OnAttackHitClip(AnimationEvent animationEvent)
    {
        if (AttackHitAudioClips.Length > 0)
        {
            var index = Random.Range(0, AttackHitAudioClips.Length);
            VoiceSource.PlayOneShot(AttackHitAudioClips[index], AudioVolume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);

        Gizmos.color = transparentRed;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.up * (AttackHeight / 2f) + transform.forward * (AttackDistance / 2f - 0.25f), Quaternion.LookRotation(transform.forward), new Vector3(AttackWidth, AttackHeight, AttackDistance + 0.5f));
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

        Gizmos.color = transparentGreen;
        Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.up * (BlockHeight / 2f) + transform.forward * 0.5f, Quaternion.LookRotation(transform.forward), new Vector3(BlockWidth, BlockHeight, 1f));
        Gizmos.DrawCube(Vector3.zero, Vector3.one);

    }
}
