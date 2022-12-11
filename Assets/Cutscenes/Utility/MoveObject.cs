using UnityEngine;

public class MoveObject : MonoBehaviour
{
    public float speed = 10f;
    
    public Transform target;
    
    private bool started = false;

    public void Update()
    {
        if (started)
        {
            transform.position = Vector3.Lerp(transform.position, target.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, speed * Time.deltaTime);
        }
    }

    public void Move()
    {
        started = true;
    }
}
