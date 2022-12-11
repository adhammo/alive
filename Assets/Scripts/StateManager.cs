using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Header("Cutscenes")]
    public GameObject WakeupCutscene;
    public GameObject DeathCutscene;
    public GameObject RebirthCutscene;
    public GameObject BoomCutscene;

    private Player _player;

    private void Awake()
    {
        // get player
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Start()
    {

        // disable all cutscenes
        WakeupCutscene.SetActive(false);
        DeathCutscene.SetActive(false);
        RebirthCutscene.SetActive(false);
        BoomCutscene.SetActive(false);

        Wakeup();
    }

    public bool rebirth;
    public bool boom;
    private void Update()
    {
        if (rebirth)
        {
            Rebirth();
            rebirth = false;
        }

        if (boom)
        {
            Boom();
            boom = false;
        }
    }

    private void Wakeup()
    {
        _player.SetAttacking(false);
        _player.DisablePlayer();

        WakeupCutscene.SetActive(true);
    }

    public void FinishWakeup()
    {
        WakeupCutscene.SetActive(false);

        _player.EnablePlayer();
    }

    public void Death()
    {
        _player.DisablePlayer();

        DeathCutscene.SetActive(true);
    }

    public void FinishDeath()
    {
        DeathCutscene.SetActive(false);

        _player.EnablePlayer();
    }

    private int deathCount = 0;
    public void Died()
    {
        deathCount++;
        if (deathCount == 1)
            Rebirth();
    }

    public void Rebirth()
    {
        _player.SetAttacking(true);
        _player.DisablePlayer();

        RebirthCutscene.SetActive(true);
    }

    public void FinishRebirth()
    {
        RebirthCutscene.SetActive(false);

        _player.EnablePlayer();
    }

    public void Boom()
    {
        _player.DisablePlayer();

        BoomCutscene.SetActive(true);
    }

    public void FinishedBoom()
    {
        BoomCutscene.SetActive(false);

        _player.EnablePlayer();
    }
}
