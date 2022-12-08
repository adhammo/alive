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
        Yelling
    }

    [Header("Animations")]
    [Tooltip("NPC interaction animation")]
    public NPCInteractions Interaction;

    private Animator _anim;

    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.SetFloat("Interaction", (int)Interaction);
    }

    void Update()
    {
        _anim.SetFloat("Interaction", (int)Interaction);
    }
}
