using UnityEngine;
using System.Collections;
using OWS.ObjectPooling;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public float damage;
    public float heat;
    public float projectileSpeed;
    public float projectileSize;

    public int burstSize; 
    public float fireDelay;
    public float fireRate;
    public float spread;

    public GameObject firePoint, projectileOBJ;
    private Projectile projectile;
    public UnitManager unitManager;

    private float lastBurst;
    public ObjectPool<PoolObject> projectilesPool;

    // Start is called before the first frame update
    void Start()
    {
        firePoint = transform.Find("FirePoint").gameObject;
        projectile = GetComponentInChildren<Projectile>();
        projectile.damage = damage;
        projectileOBJ = projectile.gameObject;
        projectilesPool = new ObjectPool<PoolObject>(projectileOBJ, 15);

        // Reset burst fire
        lastBurst = 0f;
    }
    
    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject firePoint, ObjectPool<PoolObject> objectPool)
    {
        if (Time.time > lastBurst + fireDelay)
        {
            StartCoroutine(FireBurstCoroutine(firePoint, objectPool, projectile));
            lastBurst = Time.time;
        }
    }

    // Coroutine for separate bursts
    private IEnumerator FireBurstCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, Projectile projectile)
    {
        float bulletDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, projectileSpeed);
            unitManager.heat += heat;
            yield return new WaitForSeconds(bulletDelay);
        }
    }
}
