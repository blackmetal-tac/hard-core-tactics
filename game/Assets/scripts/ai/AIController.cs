using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent unitAgent;
    private UnitManager unitManager;

    // Move parameters
    private int moveOffset = 10;    

    // Start is called before the first frame update
    void Start()
    {
        unitAgent = GetComponent<NavMeshAgent>();
        unitManager = GetComponentInChildren<UnitManager>();  
    }

    public void SetPath(GameObject target)
    {
        unitAgent.SetDestination(new Vector3(
         target.transform.position.x + Random.Range(-moveOffset, moveOffset),
         target.transform.position.y + Random.Range(-moveOffset, moveOffset),
         target.transform.position.z));

        unitManager.SetDestination(unitAgent.destination, unitAgent);
    }

    public void Move()
    {
        unitAgent.speed = unitManager.moveSpeed + 0.5f;
    }

    public void EndMove()
    {
        unitManager.moveSpeed = 0.1f;
    }
}
