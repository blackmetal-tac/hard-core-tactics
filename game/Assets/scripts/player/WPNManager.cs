using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public float recoil;
    public float spread;

    [System.Serializable]
    public class WeaponModes
    {
        public string modeName;
        public int fireMode;
    }

    public List<WeaponModes> weaponModes;

    public GameObject firePoint, projectileOBJ;
    private GameObject crosshair;
    private Projectile projectile;
    public UnitManager unitManager;

    private float lastBurst;
    public ObjectPool<PoolObject> projectilesPool;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        crosshair = GameObject.Find("Crosshair");
        firePoint = transform.Find("FirePoint").gameObject;
        projectile = GetComponentInChildren<Projectile>();
        projectile.damage = damage;
        projectileOBJ = projectile.gameObject;
        projectilesPool = new ObjectPool<PoolObject>(projectileOBJ, burstSize);

        // Reset burst fire
        lastBurst = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        // Shield regeneration
        if ((spread > 0) && gameManager.inAction)
        {
            spread -= Time.deltaTime / 5;
            float round = Mathf.Round(100f * spread) / 100f;
            spread = round;
            crosshair.transform.localScale = spread * Vector3.one;
        }
        else
        {
            spread = 0;
        }
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
            spread += recoil;
            yield return new WaitForSeconds(bulletDelay);
        }
    }
}
