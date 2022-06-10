using System.Collections;
using UnityEngine;
using OWS.ObjectPooling;

public static class ShootExtension 
{
    private static ObjectPool<PoolObject> objectsPool;
    public static float lastBurst = 0f;

    //Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public static void FireBurst(this MonoBehaviour mono, GameObject objectToSpawn, GameObject firePoint,
        float fireDelay, int burstSize, float fireRate)
    {
        if (Time.time > lastBurst + fireDelay)
        {
            mono.StartCoroutine(FireBurst(objectToSpawn, firePoint, burstSize, fireRate));
            lastBurst = Time.time;
        }
    }

    //Coroutine for separate bursts
    public static IEnumerator FireBurst(GameObject objectToSpawn, GameObject firePoint, int burstSize, 
        float fireRate)
    {        
        objectsPool = new ObjectPool<PoolObject>(objectToSpawn);

        float bulletDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {            
            objectsPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation);
            yield return new WaitForSeconds(bulletDelay);
        }
    }
}
