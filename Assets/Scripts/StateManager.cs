using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Serializable]
    public enum GameEvents
    {
        Wakeup,
    }

    [Header("Cutscenes")]
    public GameObject WakeupCutscene;
    public GameObject DeathCutscene;

    [SerializeField]
    private GameEvents _gameEvent;

    private Player _player;

    private void Start()
    {
        // get player
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // disable all cutscenes
        WakeupCutscene.SetActive(false);
        DeathCutscene.SetActive(false);

        Wakeup();
    }

    private void Wakeup()
    {
        _player.DisablePlayer();
        _player.SetAttacking(false);

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
}
