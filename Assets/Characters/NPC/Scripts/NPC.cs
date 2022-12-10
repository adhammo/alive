using System;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [Serializable]
    public enum NPCInteractions
    {
        Cheering,
        Drinking,
        Dying,
        Hip_Hop_Dancing,
        Jazz_Dancing,
        Salsa_Dancing,
        Silly_Dancing,
        Sitting_Yell,
        Swing_Dancing,
        Talking,
        Telling_A_Secret,
        Walk_In_Circle,
        Yelling,
        Sword_Shield_Idle,
        Walking1,
        Walking2,
        Walking_Female,
        Walking_Bitch,
        BreakDance,
        Sitting_Clap,
        Standing_Clap,
    }

    [Header("Animations")]
    [Tooltip("NPC interaction animation")]
    public NPCInteractions Interaction;

    private bool _active = true;

    private Animator _anim;
    private Transform _player;

    void Start()
    {
        _anim = GetComponent<Animator>();

        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _anim.SetFloat("Interaction", (int)Interaction);
    }

    void Update()
    {
        Vector3 dist = _player.position - transform.position;
        bool active = (dist.x * dist.x + dist.z * dist.z) < 100f * 100f;
        if (_active != active)
        {
            _active = active;
            _anim.enabled = _active;
            foreach (Transform child in transform)
                child.gameObject.SetActive(_active);
        }
    }
}
