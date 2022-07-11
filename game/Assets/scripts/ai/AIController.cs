using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    public NavMeshAgent unitAgent;
    private UnitManager unitManager;

    // Set objects
    public GameObject projectile, target;

    // Move parameters
    private int moveOffset = 10;    

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player");

        unitAgent = GetComponent<NavMeshAgent>();
        unitManager = GetComponentInChildren<UnitManager>();
        unitManager.target = target;     
    }

    public void SetPath()
    {
        unitAgent.SetDestination(new Vector3(
         target.transform.position.x + Random.Range(-moveOffset, moveOffset),
         target.transform.position.y + Random.Range(-moveOffset, moveOffset),
         target.transform.position.z));

        unitManager.SetDestination(unitAgent.destination, unitAgent);
    }

    public void Aim()
    {
        unitManager.aimingRightWPN.StartAim(unitManager);
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
