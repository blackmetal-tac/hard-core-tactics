using UnityEngine;
using OWS.ObjectPooling;
using System.Collections;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
    private NavMeshAgent navAgent;
    public GameObject projectile, firePoint, playerPos;    
    public int burstSize;
    public float fireDelay;
    public float fireRate;
    private float lastBurst = 0f;
    public float mechSpeed = 3.5f;
    private int moveOffset = 3;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.inAction)
        {
            FireBurst(projectile, firePoint, fireDelay, burstSize, fireRate);
            navAgent.speed = mechSpeed;
        }
        else
        {
            navAgent.speed = 0;
            SetPath();
        }
    }

    private void FireBurst(GameObject objectToSpawn, GameObject firePoint,
    float fireDelay, int burstSize, float fireRate)
    {
        if (Time.time > lastBurst + fireDelay)
        {
            StartCoroutine(FireBurst(objectToSpawn, firePoint, burstSize, fireRate));
            lastBurst = Time.time;
        }
    }

    // Coroutine for separate bursts
    private IEnumerator FireBurst(GameObject objectToSpawn, GameObject firePoint, int burstSize,
        float fireRate)
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

    private void SetPath()
    {
        navAgent.SetDestination(new Vector3(
         playerPos.transform.position.x + Random.Range(-moveOffset, moveOffset),
         playerPos.transform.position.y + Random.Range(-moveOffset, moveOffset),
         playerPos.transform.position.z));
    }
}
