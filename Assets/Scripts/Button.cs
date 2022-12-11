using UnityEngine;

public class Button : MonoBehaviour
{
    private Animator _anim;

    private bool isUsed = false;

    private StateManager _manager;

    private void Awake()
    {
        _manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<StateManager>();
    }

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player" && !isUsed)
        {
            isUsed = true;
            _anim.SetTrigger("Click");
        }
    }

    public void OnClick()
    {
        _manager.Boom();
    }
}
