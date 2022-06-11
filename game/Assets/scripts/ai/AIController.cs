using UnityEngine;
using OWS.ObjectPooling;
using System.Collections;

public class AIController : MonoBehaviour
{
    public GameObject projectile, firePoint;    
    public int burstSize;
    public float fireDelay;
    public float fireRate;
    private float lastBurst = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.inAction)
        {
            FireBurst(projectile, firePoint, fireDelay, burstSize, fireRate);
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
}
