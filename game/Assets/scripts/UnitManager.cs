using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using OWS.ObjectPooling;

public class UnitManager : MonoBehaviour
{
    private GameManager gameManager;

    // Stats    
    public float HP {get; set;}
    public float shield { get; set; }
    public float shieldRegen;
    public float heat; // Total unit heat
    public float cooling; // Cooling modifier
    public int walkDistance; // Speed (max distance)
    public float moveSpeed; // Actual agents speed
    public int burstSize; // Shooting stats
    public float fireDelay; //
    public float fireRate; //
    private float rotSpeed = 0.5f;
    public GameObject target; // Aim at this
    private Vector3 direction;

    public float shrinkTimer {get; set;}
    private float lastBurst = 0f;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Load HP and Shield bars
        this.Progress(2f, () => {
            if (shield < 1)
            { 
                shield += Time.deltaTime * 0.6f; 
            }

            if (HP < 1)
            {
                HP += Time.deltaTime * 0.6f;
            }            
        });

        // Reset burst fire
        lastBurst = 0f;        
    }

    private void FixedUpdate()
    {
        // Rotate body to target
        if (gameManager.inAction)
        {            
            direction = target.transform.position - transform.position;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(direction), Time.time * rotSpeed);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        // Shield regeneration
        if ((shield < 1) && gameManager.inAction)
        {
            shield += Time.deltaTime * shieldRegen;
            shield = Mathf.Round(shield * 100) / 100;
        }

        // Heat dissipation
        if ((heat > 0) && gameManager.inAction)
        {
            heat -= Time.deltaTime * cooling;
            heat = Mathf.Round(heat * 100) / 100;
        }

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
            firePoint.GetComponentInParent<UnitManager>().heat += objectToSpawn.GetComponent<Projectile>().heat;
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
        if (shield > 0)
        {
            shield -= damage;
        }
        else
        {
            HP -= damage;
        }             
    }
}