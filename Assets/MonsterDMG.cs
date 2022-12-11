using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterDMG : MonoBehaviour
{
    public GameObject Monster;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            other.gameObject.GetComponent<Death>().TakeDamage(20f,Monster.transform.forward);
    }
}
