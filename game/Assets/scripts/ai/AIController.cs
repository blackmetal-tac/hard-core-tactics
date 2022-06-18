using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private UnitManager unitManager;
    private GameManager gameManager;

    // Set objects
    public GameObject projectile, firePoint, target;

    // Move parameters
    private int moveOffset = 7;    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        navAgent = GetComponent<NavMeshAgent>();
        unitManager = GetComponent<UnitManager>();
        unitManager.target = target;
        navAgent.speed = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.inAction)
        {
            unitManager.FireBurst(projectile, firePoint);
            navAgent.speed = unitManager.moveSpeed + 0.5f;
        }
        else
        {
            navAgent.speed = 0;
            SetPath();
        }
    }

    private void SetPath()
    {
        navAgent.SetDestination(new Vector3(
         target.transform.position.x + Random.Range(-moveOffset, moveOffset),
         target.transform.position.y + Random.Range(-moveOffset, moveOffset),
         target.transform.position.z));

        unitManager.SetDestination(navAgent.destination, navAgent);
    }
}
