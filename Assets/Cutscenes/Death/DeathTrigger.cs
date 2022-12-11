using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private StateManager _manager;

    private void Start()
    {
        _manager = GameObject.FindWithTag("Manager").GetComponent<StateManager>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") {
            _manager.Death();
        }
    }
}
