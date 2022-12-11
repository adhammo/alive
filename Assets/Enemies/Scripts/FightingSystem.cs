using System.Collections;
using UnityEngine;

public class FightingSystem : MonoBehaviour
{
    public enum FighterTypes
    {
        Spearman,
        Monster
    }

    public float Range = 20f;
    public FighterTypes FighterType;
    public GameObject SpearPrefab;

    // animator hashes
    private int _throwAnimHash = Animator.StringToHash("Throw");
    private int _headbuttAnimHash = Animator.StringToHash("Headbutt");

    private GameObject _player;
    private Vector3 _targetPoint = Vector3.zero;
    private Animator _anim;

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
                    break;

                case FighterTypes.Monster:
                    if ((_targetPoint != Vector3.zero) && (Vector3.Distance(transform.position, _targetPoint) >= 2.0f))
                    {
                        _anim.SetBool(_headbuttAnimHash, true);
                        transform.Translate(Vector3.forward * Time.deltaTime * 10);
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
    }

    public IEnumerator Throw()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);
            if (Vector3.Distance(transform.position, _player.transform.position) < Range)
                _anim.SetTrigger(_throwAnimHash);
        }
    }

    public IEnumerator Headbutt()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            if ((Vector3.Distance(transform.position, _player.transform.position) < Range) && _targetPoint == Vector3.zero)
                _targetPoint = _player.transform.position;
        }
    }

    private void OnSpearCallback()
    {
        GameObject CurrentSpear = Instantiate(SpearPrefab, new Vector3(transform.position.x, transform.position.y + 0.8f, transform.position.z), Quaternion.identity);
        CurrentSpear.transform.LookAt(_player.transform);
    }
}
