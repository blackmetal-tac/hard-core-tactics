using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent unitAgent;
    private UnitManager unitManager;
    public GameObject target;

    // Move parameters
    private readonly int moveOffset = 15;

    // Start is called before the first frame update
    void Start()
    {
        unitAgent = GetComponent<NavMeshAgent>();
        unitManager = GetComponentInChildren<UnitManager>();
    }

    public void SetPath(NavMeshAgent target)
    {
        unitAgent.stoppingDistance = 0f;
        unitAgent.SetDestination(new Vector3(
         target.transform.position.x + Random.Range(-moveOffset, moveOffset),
         target.transform.position.y + Random.Range(-moveOffset, moveOffset),
         target.transform.position.z));

        // Get distance between target to avoid collision
        float targetDistance = Vector3.Distance(unitAgent.destination, target.transform.position);
        float targetDestDist = Vector3.Distance(unitAgent.destination, target.destination);

        if (unitAgent.destination == target.transform.position || targetDistance < 1f 
            || targetDestDist < 1f)
        {
            unitAgent.stoppingDistance = 1f;
        }
        unitManager.SetDestination(unitAgent.destination, unitAgent);
    }

    public void Move()
    {
        unitAgent.speed = unitManager.moveSpeed + 0.5f;
        // Change fire modes depending on heat or enable weapon after overheat
        foreach (WPNManager weapon in unitManager.weaponList)
        {
            if (weapon != null && weapon.downTimer <= 0 && unitManager.heat < unitManager.heatTreshold)
            {
                int changeMode = Random.Range(1, weapon.weaponModes.Count);
                weapon.burstSize = weapon.weaponModes[changeMode].fireMode;
            }
            else if (weapon != null && weapon.downTimer <= 0 && unitManager.heat >= unitManager.heatTreshold)
            {
                int changeMode = Random.Range(0, weapon.weaponModes.Count - 1);
                weapon.burstSize = weapon.weaponModes[changeMode].fireMode;
            }
        }        
    }
    public void EndMove()
    {
        unitManager.moveSpeed = 0.1f;
        unitManager.UpdateTimer();
    }
}
