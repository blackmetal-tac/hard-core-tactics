using System.Collections;
using UnityEngine;

public static class ShootExtension 
{
    public static float lastBurst = 0f;
    //Set ObjectPooler, projectile NAME, Fire Point, delay between bursts, number of shots, fire rate
    public static void FireBurst(this MonoBehaviour mono, ObjectPooler objectPooler, string projectile, GameObject firePoint,
        float fireDelay, int burstSize, float fireRate)
    {
        if (Time.time > lastBurst + fireDelay)
        {
            mono.StartCoroutine(FireBurst(objectPooler, firePoint, projectile, burstSize, fireRate));
            lastBurst = Time.time;
        }
    }

    //Coroutine for separate bursts
    public static IEnumerator FireBurst(ObjectPooler objectPooler, GameObject firePoint, string projectile, int burstSize, 
        float fireRate)
    {
        float bulletDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectPooler.SpawnFromPool(projectile, firePoint.transform.position, firePoint.transform.rotation);
            yield return new WaitForSeconds(bulletDelay);
        }
    }
}
