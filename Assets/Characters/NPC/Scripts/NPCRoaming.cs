using UnityEngine;
using UnityEngine.AI;

public class NPCRoaming : MonoBehaviour
{
    /* AI Variables */
    public GameObject[] RandomPoints;

    /* FSM Variables */
    private int index;
    private Vector3 direction = Vector3.zero;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        index = GetRoamingPosition();
    }

    // Update is called once per frame
    void Update()
    {
        Roaming();
    }

    /*Function to Face the point at roaming state*/
    void FaceTarget_point(Transform point)
    {
        Vector3 direction = point.position - transform.position;
        if (direction.x != 0 && direction.z != 0)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }

    public static Vector3 GetRandomDir()
    {
        return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    private int GetRoamingPosition()
    {
        return Random.Range(0, RandomPoints.Length);
    }

    private void Roaming()
    {
        agent.SetDestination(RandomPoints[index].transform.position);

        if (Vector3.Distance(transform.position, RandomPoints[index].transform.position) < 3f)
        {
            index = GetRoamingPosition();
        }
    }
}

