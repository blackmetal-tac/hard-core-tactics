using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWS.ObjectPooling;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public float damage, heat, projectileSpeed, projectileSize, fireDelay, fireRate, recoil;
    public int burstSize;
    private float spread;
    private readonly float spreadMult = 0.5f;
    [HideInInspector] public int downTimer;
    [HideInInspector] public float lastBurst;

    [System.Serializable]
    public class WeaponModes
    {
        public string modeName;
        public int fireMode;
    }

    public List<WeaponModes> weaponModes;

    public GameObject firePoint, projectileOBJ;
    private Projectile projectile;
    private Missile missile;
    public UnitManager unitManager;

    public ObjectPool<PoolObject> projectilesPool;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        firePoint = transform.Find("FirePoint").gameObject;

        projectile = GetComponentInChildren<Projectile>();        
        if (projectile != null)
        {
            projectile.damage = damage;
            projectileOBJ = projectile.transform.parent.gameObject;
        }
        else 
        {
            missile = transform.Find("Missile").GetComponentInChildren<Missile>();
            missile.damage = damage;
            projectileOBJ = missile.transform.parent.gameObject;
        }
        
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
            if (projectile != null)
            {
                StartCoroutine(FireBurstCoroutine(firePoint, projectilesPool));
                lastBurst = Time.time;
            }
            else 
            {
                StartCoroutine(FireMissilesCoroutine(firePoint, projectilesPool, target.transform.position));
                lastBurst = Time.time + gameManager.turnTime - fireDelay; // fire burst once (increase delay)
            }            
        }        
    }


    // Coroutine for separate bursts
    private IEnumerator FireBurstCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool)
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

    // Coroutine for missiles
    private IEnumerator FireMissilesCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, Vector3 target)
    {
        float bulletDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, projectileSpeed, target);

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
