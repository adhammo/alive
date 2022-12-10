using System;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    [Serializable]
    public enum GameEvents
    {
        Wakeup,
    }

    [SerializeField]
    private GameEvents _gameEvent = GameEvents.Wakeup;

    private void Start()
    {

    }

    private void Update()
    {
        switch (_gameEvent)
        {
            case GameEvents.Wakeup:
                StartWakeup();
                break;
        }
    }

    private void StartWakeup()
    {
        
    }

    private void FinishedWakeup()
    {

    }
}
