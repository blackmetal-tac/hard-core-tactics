using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWS.ObjectPooling;
using DG.Tweening;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public enum ProjectileType { Bullet, Missile, Laser, AMS }
    public ProjectileType projectileType;
    public bool homing;
    [Range(0, 15)] public int radiusAMS, projectileSpeed;
    [Range(0, 0.5f)] public float projectileSize, heat, recoil;
    [Range(0, 1)] public float damage, fireDelay;    
    [Range(0, 3000)] public float fireRate, laserRange;
    [HideInInspector] public int burstSize,downTimer;
    [HideInInspector] public float lastBurst;
    private readonly float spreadMult = 0.5f;
    private Vector3 spreadVector;

    private float spread, updateTimer, laserWidth;
    private readonly float delay = 0.1f;

    [System.Serializable]
    public class WeaponModes
    {
        public string modeName;
        public int fireMode;
    }
    public List<WeaponModes> weaponModes;

    private Transform tubesContainer;
    [HideInInspector] public List<Transform> tubes;

    [HideInInspector] public GameObject firePoint, projectileOBJ, targetAMS;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public bool isFriend;
    private GameManager gameManager;
    private Collider colliderAMS;

    private LineRenderer lineRenderer;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        firePoint = transform.Find("FirePoint").gameObject;
        updateTimer = Time.fixedTime + delay;

        // Set AMS parameters 
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
        else 
        {
            radiusAMS = 0;
        }

        if (projectileType == ProjectileType.Missile)
        {
            // Count tubes to fire missile from each
            tubesContainer = transform.Find("Tubes");
            for (int i = 0; i < tubesContainer.childCount; i++)
            {
                tubes.Add(tubesContainer.GetChild(i));
            }
        }

        // Friend/Foe system for AMS to intercept
        if (projectileType == ProjectileType.Missile && unitManager.transform.parent.parent.name == "PlayerSquad")
        {
            isFriend = true;
        }

        if (projectileType == ProjectileType.Laser)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
            projectileSpeed = 0;
            recoil = 0;
        }
    }

    void FixedUpdate()
    {
        // Timer to slow update
        if (Time.fixedTime >= updateTimer)
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
            updateTimer = Time.fixedTime + delay;
        }

        if (projectileType == ProjectileType.Laser)
        {

            lineRenderer.SetPosition(0, firePoint.transform.position); 
            if (Physics.Raycast(firePoint.transform.position, firePoint.transform.forward, out hit, laserRange))
            {
                lineRenderer.SetPosition(1, hit.point);
            }
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject target)
    {        
        if (projectileType != ProjectileType.Laser)
        {
            spreadVector = new(
                Random.Range((-unitManager.moveSpeed * spreadMult) - spread, (unitManager.moveSpeed * spreadMult) + spread),
                Random.Range((-unitManager.moveSpeed * spreadMult) - spread / 2, (unitManager.moveSpeed * spreadMult) + spread / 2),
                Random.Range((-unitManager.moveSpeed * spreadMult) - spread, (unitManager.moveSpeed * spreadMult) + spread));
        }

        if (projectileType != ProjectileType.AMS && !homing)
        {
            firePoint.transform.LookAt(target.transform.position + spreadVector);
        }
        else if (projectileType != ProjectileType.Laser)
        {
            firePoint.transform.LookAt(target.transform.position);
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
            else if (projectileType == ProjectileType.Missile && !homing)
            {
                StartCoroutine(FireMissilesCoroutine(firePoint, gameManager.missilesPool, target.transform.position));
                lastBurst = Time.time + gameManager.turnTime - fireDelay; // fire burst once (increase delay)
            }
            else if (projectileType == ProjectileType.Missile && homing)
            {
                StartCoroutine(FireHMissilesCoroutine(firePoint, gameManager.missilesPool, target));
                lastBurst = Time.time + gameManager.turnTime - fireDelay; // fire burst once (increase delay)
            }
            else if (projectileType == ProjectileType.AMS && targetAMS != null)
            {                
                StartCoroutine(FireBurstCoroutine(firePoint, gameManager.amsPool));
                lastBurst = Time.time;
            }
            else if (projectileType == ProjectileType.Laser)
            {
                StartCoroutine(FireLaserCoroutine());
                lastBurst = Time.time;
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
            firePoint.transform.position = tubes[i].position;
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, damage, projectileSpeed, target, isFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for homing missiles
    private IEnumerator FireHMissilesCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, GameObject target)
    {
        float shotDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            firePoint.transform.position = tubes[i].position;
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, projectileSize, damage, projectileSpeed, target, isFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private IEnumerator FireLaserCoroutine()
    {
        bool grow = true;
        float shotDelay = 60 / fireRate;
        for (int i = 0; i < burstSize; i++)
        {
            
            if (laserWidth < 0.03f && grow)
            {
                laserWidth += 0.01f;
            }
            else if (laserWidth > 0 && !grow)
            {
                laserWidth -= 0.01f;
            }
            lineRenderer.startWidth = laserWidth;
            lineRenderer.endWidth = laserWidth;

            if (hit.collider.name == "Body")
            { 
                hit.collider.GetComponent<UnitManager>().TakeDamage(damage);
            }
            HeatRecoil();

            this.Wait(0.05f, () =>
            {
                grow = false;
            });

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
