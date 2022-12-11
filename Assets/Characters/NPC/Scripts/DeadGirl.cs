using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadGirl : MonoBehaviour
{
    private bool _active = true;

    private Transform _player;
    public GameObject box;

    public float distance = 10;


    void Start()
    {
                box.SetActive(false);
                _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void LateUpdate()
    {

        if ( Vector3.Distance(box.transform.position , _player.position) < distance )
        {
            box.SetActive(true);
        }
    }
}
