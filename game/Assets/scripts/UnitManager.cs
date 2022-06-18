using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using OWS.ObjectPooling;

public class UnitManager : MonoBehaviour
{
    private GameManager gameManager;

    // Stats
    public float HP {get; set;}
    public int walkDistance; // Speed (max distance)
    public float moveSpeed; // Actual agents speed
    public int burstSize; // Shooting stats
    public float fireDelay; //
    public float fireRate; //
    public GameObject target; // Aim at this

    public float shrinkTimer {get; set;}
    private float lastBurst = 0f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        HP = 1f;

        lastBurst = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Death
        if (HP <= 0)
        {
            this.transform.localScale = Vector3.zero;
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject objectToSpawn, GameObject firePoint)
    {
        if (Time.time > lastBurst + fireDelay)
        {
            StartCoroutine(FireBurstCoroutine(objectToSpawn, firePoint));
            lastBurst = Time.time;
        }
    }

    // Coroutine for separate bursts
    private IEnumerator FireBurstCoroutine(GameObject objectToSpawn, GameObject firePoint)
    {
        ObjectPool<PoolObject> objectsPool;
        objectsPool = new ObjectPool<PoolObject>(objectToSpawn);

        float bulletDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectsPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation);
            yield return new WaitForSeconds(bulletDelay);
        }
    }

    // Set move position and maximum move distance (speed)
    public void SetDestination(Vector3 target, NavMeshAgent navAgent)
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.speed = 0;

        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;
            if (segmentDistance <= walkDistance)
            {
                navAgent.SetDestination(target);
                moveSpeed = segmentDistance / gameManager.turnTime;
            }
            else
            {
                Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * walkDistance);
                NavMesh.CalculatePath(transform.position, finalPoint, NavMesh.AllAreas, path);
                navAgent.SetPath(path);
                moveSpeed = walkDistance / gameManager.turnTime;
                break;
            }
        }
    }

    // Take damage
    public void TakeDamage(float damage)
    {
        HP -= damage;
    }
}
