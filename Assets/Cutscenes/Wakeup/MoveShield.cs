using UnityEngine;

public class MoveShield : MonoBehaviour
{
    public float speed = 10f;
    
    private Transform _shield;

    private bool started = false;

    public void Start()
    {
        _shield = GameObject.FindWithTag("Player").GetComponent<Fighter>().ShieldGameObject.GetComponentInParent<Transform>();
    }

    public void Update()
    {
        if (started)
        {
            transform.position = Vector3.Lerp(transform.position, _shield.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, _shield.rotation, speed * Time.deltaTime);
        }
    }

    public void Move()
    {
        started = true;
    }
}
