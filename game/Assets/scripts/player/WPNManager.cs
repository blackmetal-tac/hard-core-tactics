using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWS.ObjectPooling;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public float damage, heat, projectileSpeed, projectileSize, fireDelay, fireRate, recoil;
    public int burstSize;
    private float spread, lastBurst = 0f;
    private readonly float spreadMult = 0.5f;

    public int downTimer { get; set; }

    [System.Serializable]
    public class WeaponModes
    {
        public string modeName;
        public int fireMode;
    }

    public List<WeaponModes> weaponModes;

    public GameObject firePoint, projectileOBJ;
    private Projectile projectile;
    public UnitManager unitManager;

    public ObjectPool<PoolObject> projectilesPool;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        firePoint = transform.Find("FirePoint").gameObject;
        projectile = GetComponentInChildren<Projectile>();
        projectile.damage = damage;
        projectileOBJ = projectile.gameObject;
        projectilesPool = new ObjectPool<PoolObject>(projectileOBJ, burstSize);
    }

    // Update is called once per frame
    void Update()
    {  
        // Bullet spread
        if ((spread > 0) && gameManager.inAction)
        {
            spread -= Time.deltaTime / 3; 
            unitManager.spread = spread;           
        }
        else
        {
            spread = 0;
            unitManager.spread = spread;
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject target)
    {
        Vector3 spreadVector = new Vector3(
            Random.Range((-unitManager.moveSpeed * spreadMult) - spread, (unitManager.moveSpeed * spreadMult) + spread),
            Random.Range((-unitManager.moveSpeed * spreadMult) - spread / 2, (unitManager.moveSpeed * spreadMult) + spread / 2),
            Random.Range((-unitManager.moveSpeed * spreadMult) - spread, (unitManager.moveSpeed * spreadMult) + spread));
        firePoint.transform.LookAt(target.transform.position + spreadVector);

        if (Time.time > lastBurst + fireDelay)
        {
            StartCoroutine(FireBurstCoroutine(firePoint, projectilesPool, projectile));
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

            if (unitManager.heat < 1)
            {
                unitManager.heat += heat;
            }

            if (spread < 1)
            {
                spread += recoil;
            }            

            yield return new WaitForSeconds(bulletDelay);
        }
    }
}
