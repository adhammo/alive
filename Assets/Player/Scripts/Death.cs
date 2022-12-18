using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Fighter))]
public class Death : MonoBehaviour
{
    [Header("Stats")]
    [Tooltip("Player max health")]
    public float MaxHealth = 100.0f;

    [Header("Disable")]
    [Tooltip("Scripts to disable")]
    public MonoBehaviour[] DisabledScripts;

    // [Header("Hurt")]
    // [Tooltip("Hurt UI object")]
    // public Hurt HurtUI;

    [Header("Sounds")]
    [Tooltip("Voice audio source")]
    public AudioSource VoiceSource;
    [Tooltip("Hurt audio clips")]
    public AudioClip[] HurtAudioClips;
    [Tooltip("Impact audio clips")]
    public AudioClip[] ImpactAudioClips;
    [Tooltip("Death audio clip")]
    public AudioClip DeathAudioClip;
    [Tooltip("Audio effects volume")]
    [Range(0, 1)] public float AudioVolume = 0.5f;

    [SerializeField]
    [Tooltip("Player current health")]
    private float _currentHealth;

    [HideInInspector()]
    public bool CanHurt = false;

    public float CurrentHealth { get { return _currentHealth; } }

    // animator hashes
    private int _hurtAnimHash = Animator.StringToHash("Hurt");
    private int _hurtValAnimHash = Animator.StringToHash("HurtVal");
    private int _impactAnimHash = Animator.StringToHash("Impact");

    private CharacterController _controller;
    private Animator _anim;
    private Fighter _fighter;
    private StateManager _manager;

    private bool initialized = false;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _fighter = GetComponent<Fighter>();

        _manager = GameObject.FindWithTag("Manager").GetComponent<StateManager>();

        initialized = true;
        Reset();

        _currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage)
    {
        Hurt();

        _currentHealth -= damage;
        if (_currentHealth <= 0.0f)
            Die();
    }

    public void TakeDamage(float damage, Vector3 direction)
    {
        if (_fighter.IsBlocked(direction))
        {
            Impact();
            _currentHealth -= damage * _fighter.BlockReduction;
        }
        else
        {
            Hurt();
            _currentHealth -= damage;
        }

        if (_currentHealth <= 0.0f)
            Die();
    }

    private void Hurt()
    {
        if (CanHurt)
        {
            var index = Random.Range(0, 2);
            _anim.SetFloat(_hurtValAnimHash, index);
            _anim.SetTrigger(_hurtAnimHash);
            // HurtUI.GetHurt();
        }
    }

    private void Impact()
    {
        if (CanHurt)
        {
            _anim.SetTrigger(_impactAnimHash);
            // HurtUI.GetHurt();
        }
    }

    private void Die()
    {
        VoiceSource.PlayOneShot(DeathAudioClip);
        _manager.Died();
    }

    public void Reset()
    {
        if (initialized)
        {
            _anim.ResetTrigger(_hurtAnimHash);
            _anim.ResetTrigger(_impactAnimHash);
        }
    }

    private void OnHurtClip(AnimationEvent animationEvent)
    {
        if (HurtAudioClips.Length > 0)
        {
            var index = Random.Range(0, HurtAudioClips.Length);
            VoiceSource.PlayOneShot(HurtAudioClips[index], AudioVolume);
        }
    }

    private void OnImpactClip(AnimationEvent animationEvent)
    {
        if (ImpactAudioClips.Length > 0)
        {
            var index = Random.Range(0, ImpactAudioClips.Length);
            AudioSource.PlayClipAtPoint(ImpactAudioClips[index], _fighter.ShieldGameObject.transform.position, AudioVolume);
        }
    }
}
