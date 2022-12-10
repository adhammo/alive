using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Serializable]
    public enum GameEvents
    {
        Wakeup,
    }

    [Header("Wakeup")]
    public GameObject WakeupCutscene;

    [SerializeField]
    private GameEvents _gameEvent;

    private Player _player;

    private void Start()
    {
        // get player
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        // disable all cutscenes
        WakeupCutscene.SetActive(false);

        Wakeup();
    }

    private void Wakeup()
    {
        _player.DisablePlayer();
        // _player.SetAttacking(false);

        WakeupCutscene.SetActive(true);
    }

    public void FinishWakeup()
    {
        WakeupCutscene.SetActive(false);

        _player.EnablePlayer();
    }
}
