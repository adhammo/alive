using UnityEngine;

public class DeadGirl : MonoBehaviour
{
    private Transform _player;
    public GameObject box;

    public float distance = 10;

    void Awake()
    {
        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Start()
    {
        box.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Vector3.SqrMagnitude(box.transform.position - _player.position) < distance * distance)
            box.SetActive(true);
    }
}
