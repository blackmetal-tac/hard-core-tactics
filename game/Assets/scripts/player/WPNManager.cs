using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWS.ObjectPooling;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public enum ProjectileType { Bullet, Missile, AMS }

    public ProjectileType projectileType;
    public float damage, heat, projectileSpeed, projectileSize, fireDelay, fireRate, recoil, radiusAMS;
    private float spread;
    private readonly float spreadMult = 0.5f;
    [HideInInspector] public int burstSize,downTimer;
    [HideInInspector] public float lastBurst;

    [System.Serializable]
    public class WeaponModes
    {
        public string modeName;
        public int fireMode;
    }
    public List<WeaponModes> weaponModes;

    [HideInInspector] public GameObject firePoint, projectileOBJ;
    [HideInInspector] public UnitManager unitManager;
    private GameManager gameManager;
    private Collider colliderAMS;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        firePoint = transform.Find("FirePoint").gameObject;

        if (projectileType == ProjectileType.AMS)
        {
            colliderAMS = GetComponentInChildren<Collider>();
            colliderAMS.enabled = true;
            colliderAMS.transform.localScale = radiusAMS * Vector3.one;
        }
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
            if (projectileType == ProjectileType.Bullet)
            {
                StartCoroutine(FireBurstCoroutine(firePoint, gameManager.bulletsPool));
                lastBurst = Time.time;
            }
            else if (projectileType == ProjectileType.Missile)
            {
                StartCoroutine(FireMissilesCoroutine(firePoint, gameManager.missilesPool, target.transform.position));
                lastBurst = Time.time + gameManager.turnTime - fireDelay; // fire burst once (increase delay)
            }
            else if (projectileType == ProjectileType.AMS)
            {
                GameObject targetMissile = colliderAMS.gameObject;
                StartCoroutine(FireMissilesCoroutine(firePoint, gameManager.bulletsPool, targetMissile.transform.position));
                lastBurst = Time.time;
            }
        }        
    }


    // Coroutine for separate bursts
    private IEnumerator FireBurstCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool)
    {
        float bulletDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, damage, projectileSpeed);
          
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
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, damage, projectileSpeed, target);

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
