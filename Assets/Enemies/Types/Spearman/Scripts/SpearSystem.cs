using System.Collections;
using UnityEngine;

public class SpearSystem : MonoBehaviour
{
    public float SpearSpeed = 2;
    private bool stick = false;
    public GameObject SpearMan;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (!stick) GetComponent<Rigidbody>().velocity = transform.forward * SpearSpeed;
        else GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("should stick with player");
            stick = true;
            transform.parent = other.gameObject.transform;
            other.gameObject.GetComponent<Death>().TakeDamage(20f, SpearMan.transform.forward);
            Destroy(gameObject, 1f);
        }
    }
}
