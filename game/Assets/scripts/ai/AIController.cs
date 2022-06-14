using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    private UnitManager unitManager;
    private GameManager gameManager;

    // Set objects
    public GameObject projectile, firePoint, playerPos;

    // Set stats
    public int burstSize;
    public float fireDelay;
    public float fireRate;
    public float mechSpeed = 3.5f;

    // Move parameters
    private int moveOffset = 7;    

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        navAgent = GetComponent<NavMeshAgent>();
        unitManager = GetComponent<UnitManager>();
        navAgent.speed = 0;        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.inAction)
        {
            unitManager.FireBurst(projectile, firePoint, fireDelay, burstSize, fireRate);
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
         playerPos.transform.position.x + Random.Range(-moveOffset, moveOffset),
         playerPos.transform.position.y + Random.Range(-moveOffset, moveOffset),
         playerPos.transform.position.z));

        unitManager.SetDestination(navAgent.destination, navAgent);
    }
}
