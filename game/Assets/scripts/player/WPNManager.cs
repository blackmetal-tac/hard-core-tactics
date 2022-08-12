using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWS.ObjectPooling;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public enum ProjectileType { Bullet, Missile, AMS }
    public ProjectileType projectileType;
    [Range(0, 0.5f)] public float projectileSize, heat, recoil;
    [Range(0, 1)] public float damage, fireDelay;
    [Range(0, 15)] public int radiusAMS, projectileSpeed;
    [Range(0, 3000)] public float fireRate;
    [HideInInspector] public int burstSize,downTimer;
    [HideInInspector] public float lastBurst;
    private readonly float spreadMult = 0.5f;
    
    private float spread, timer;
    private readonly float delay = 0.1f;

    [System.Serializable]
    public class WeaponModes
    {
        public string modeName;
        public int fireMode;
    }
    public List<WeaponModes> weaponModes;

    [HideInInspector] public GameObject firePoint, projectileOBJ, targetAMS;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public bool isFriend;
    private GameManager gameManager;
    private Collider colliderAMS;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        firePoint = transform.Find("FirePoint").gameObject;
        timer = Time.fixedTime + delay;

        if (projectileType == ProjectileType.AMS)
        {
            damage = 0;
            colliderAMS = GetComponentInChildren<Collider>();
            if (unitManager.transform.parent.parent.name == "PlayerSquad")
            {
                colliderAMS.gameObject.layer = 14;
            }
            else
            {
                colliderAMS.gameObject.layer = 15;
            }
            colliderAMS.enabled = true;
            colliderAMS.transform.localScale = radiusAMS * Vector3.one;
        }

        if (projectileType == ProjectileType.Missile && unitManager.transform.parent.parent.name == "PlayerSquad")
        {
            isFriend = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Timer to slow update
        if (Time.fixedTime >= timer)
        {
            // Bullet spread 
            if (unitManager.transform.parent.name == "Player" && unitManager.spread < 2
                && gameManager.inAction && projectileType != ProjectileType.AMS)
            {
                unitManager.spread += spread / 50;
            }

            if (spread > 0)
            {
                spread -= Time.fixedDeltaTime * 2;
                unitManager.spread = spread;
            }
            else
            {
                spread = 0;
                unitManager.spread = spread;
            }
            timer = Time.fixedTime + delay;
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject target)
    {
        Vector3 spreadVector = new(
                Random.Range((-unitManager.moveSpeed * spreadMult) - spread, (unitManager.moveSpeed * spreadMult) + spread),
                Random.Range((-unitManager.moveSpeed * spreadMult) - spread / 2, (unitManager.moveSpeed * spreadMult) + spread / 2),
                Random.Range((-unitManager.moveSpeed * spreadMult) - spread, (unitManager.moveSpeed * spreadMult) + spread));

        if (projectileType != ProjectileType.AMS)
        {
            firePoint.transform.LookAt(target.transform.position + spreadVector);
        }
        if (targetAMS != null)
        {
            firePoint.transform.LookAt(targetAMS.transform.position + spreadVector);
            if (targetAMS.transform.parent.localScale == Vector3.zero)
            {
                targetAMS = null;
            }
        }

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
            else if (projectileType == ProjectileType.AMS && targetAMS != null)
            {                
                StartCoroutine(FireBurstCoroutine(firePoint, gameManager.bulletsPool));
                lastBurst = Time.time;// && targetAMS != null
            }
        }        
    }


    // Coroutine for separate bursts
    private IEnumerator FireBurstCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool)
    {
        float shotDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, damage, projectileSpeed);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for missiles
    private IEnumerator FireMissilesCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, Vector3 target)
    {
        float shotDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, damage, projectileSpeed, target, isFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private void HeatRecoil()
    {
        if (unitManager.heat < 1)
        {
            unitManager.heat += heat;
        }

        if (spread < 1)
        {
            spread += recoil;
        }
    }
}
