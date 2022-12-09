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
    [Tooltip("Hurt audio clip")]
    public AudioClip ImpactAudioClip;
    [Tooltip("Hurt audio clip")]
    public AudioClip HurtAudioClip;
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
    private int _impactAnimHash = Animator.StringToHash("Impact");

    private CharacterController _controller;
    private Animator _anim;
    private Fighter _fighter;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
        _fighter = GetComponent<Fighter>();

        _currentHealth = MaxHealth;
    }

    public void OnInteract()
    {
        TakeDamage(10f);
    }

    public void OnClick()
    {
        TakeDamage(10f, -transform.forward);
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

    public void Reset()
    {
        foreach (var script in DisabledScripts)
            script.enabled = true;

        _controller.enabled = true;

        _currentHealth = MaxHealth;
    }

    private void Hurt()
    {
        if (CanHurt)
        {
            VoiceSource.PlayOneShot(HurtAudioClip);
            _anim.SetTrigger(_hurtAnimHash);
            // HurtUI.GetHurt();
        }
    }

    private void Impact()
    {
        if (CanHurt)
        {
            VoiceSource.PlayOneShot(ImpactAudioClip);
            _anim.SetTrigger(_impactAnimHash);
            // HurtUI.GetHurt();
        }
    }

    private void Die()
    {
        VoiceSource.PlayOneShot(DeathAudioClip);

        foreach (var script in DisabledScripts)
            script.enabled = false;

        _controller.enabled = false;
    }
}
