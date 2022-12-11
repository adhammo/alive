using System.Collections;
using UnityEngine;

public class FightingSystem : MonoBehaviour
{
    public enum FighterTypes
    {
        Spearman,
        Monster
    }

    public float Health = 100f, Range = 20f;
    public FighterTypes FighterType;
    public GameObject SpearPrefab;

    // animator hashes
    private int _throwAnimHash = Animator.StringToHash("Throw");
    private int _headbuttAnimHash = Animator.StringToHash("Headbutt");
    private int _SpearManWalking = Animator.StringToHash("SpearManWalking");

    private GameObject _player;
    private Vector3 _targetPoint = Vector3.zero, _EnemyRef = Vector3.zero;
    private Animator _anim;
    public bool EnemyMovement = true;

    public AudioSource VoiceSource;
    public AudioClip[] audioClips;

    public bool CanAttack = false;

    public void EnableAttack()
    {
        CanAttack = true;
    }

    public void DisableAttack()
    {
        CanAttack = false;
    }

    private void Awake()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();

        switch (FighterType)
        {
            case FighterTypes.Spearman:
                StartCoroutine(Throw());
                _EnemyRef = transform.position;
                break;

            case FighterTypes.Monster:
                StartCoroutine(Headbutt());
                break;

            default:
                Debug.Log("fighter type is undetected");
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < Range)
        {
            switch (FighterType)
            {
                case FighterTypes.Spearman:
                    transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
                    if (EnemyMovement)
                    {
                        if (Vector3.Distance(transform.position, _EnemyRef) <= 10.0f)
                        {
                            _anim.SetBool(_SpearManWalking, true);
                            transform.Translate(Vector3.forward * Time.deltaTime * 1);
                        }
                        else
                            _anim.SetBool(_SpearManWalking, false);
                    }
                    break;

                case FighterTypes.Monster:
                    if ((_targetPoint != Vector3.zero) && (Vector3.Distance(transform.position, _targetPoint) >= 0.5f))
                    {
                        _anim.SetBool(_headbuttAnimHash, true);
                        transform.Translate(Vector3.forward * Time.deltaTime * 5);
                        transform.LookAt(_targetPoint);
                    }
                    else
                    {
                        _anim.SetBool(_headbuttAnimHash, false);
                        _targetPoint = Vector3.zero;
                        transform.LookAt(new Vector3(_player.transform.position.x, transform.position.y, _player.transform.position.z));
                    }
                    break;

                default:
                    Debug.Log("fighter type is undetected");
                    break;
            }
        }
        else
        {
            if ((FighterType == FighterTypes.Spearman) && (EnemyMovement))
            {
                if (Vector3.Distance(transform.position, _EnemyRef) >= 2.0f)
                {
                    transform.LookAt(new Vector3(_EnemyRef.x, transform.position.y, _EnemyRef.z));
                    _anim.SetBool(_SpearManWalking, true);
                    transform.Translate(Vector3.forward * Time.deltaTime * 1);
                }
                else
                    _anim.SetBool(_SpearManWalking, false);
            }

        }

    }

    public IEnumerator Throw()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.5f);
            if (CanAttack && Vector3.Distance(transform.position, _player.transform.position) < Range)
                _anim.SetBool(_throwAnimHash, true);
            else
                _anim.SetBool(_throwAnimHash, false);
        }
    }

    public IEnumerator Headbutt()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            if (CanAttack && (Vector3.Distance(transform.position, _player.transform.position) < Range) && _targetPoint == Vector3.zero)
                _targetPoint = _player.transform.position;
        }
    }

    private void OnSpearCallback()
    {
        GameObject CurrentSpear = Instantiate(SpearPrefab, new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), Quaternion.identity);
        CurrentSpear.GetComponent<SpearSystem>().SpearMan = this.gameObject;
        CurrentSpear.transform.LookAt(new Vector3(_player.transform.position.x, _player.transform.position.y + 1, _player.transform.position.z));
    }

    public void TakeDmg(float DmgAmount)
    {
        Health = Health - DmgAmount;
        VoiceSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length - 1)]);
        if (Health <= 0)
        {
            if (Random.Range(0, 1) == 0)
                _anim.SetBool("React1", true);
            else
                _anim.SetBool("React2", true);
        }
        else
            _anim.SetBool("Died", true);
    }
}
