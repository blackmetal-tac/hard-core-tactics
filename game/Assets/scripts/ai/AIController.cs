using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private UnitManager unitManager;
    private GameManager gameManager;

    // Set objects
    public GameObject projectile, target;

    // Move parameters
    private int moveOffset = 10;    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        target = GameObject.Find("Player");

        navAgent = GetComponent<NavMeshAgent>();
        unitManager = GetComponentInChildren<UnitManager>();
        unitManager.target = target;
        navAgent.speed = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.inAction)
        {
            unitManager.FireBurst(unitManager.firePoint, gameManager.bulletsPool);
            navAgent.speed = unitManager.moveSpeed + 0.5f;
        }
        else
        {
            navAgent.speed = 0;
        }
    }

    public void SetPath()
    {
        navAgent.SetDestination(new Vector3(
         target.transform.position.x + Random.Range(-moveOffset, moveOffset),
         target.transform.position.y + Random.Range(-moveOffset, moveOffset),
         target.transform.position.z));

        unitManager.SetDestination(navAgent.destination, navAgent);
    }
}
