using UnityEngine;

public class SceneManager : MonoBehaviour
{
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;

    public void Start()
    {
        SetCursorState(cursorLocked);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}