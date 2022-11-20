using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using OWS.ObjectPooling;
using DG.Tweening;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    private enum ProjectileType { Bullet, Missile, Laser, AMS }
    [SerializeField] private ProjectileType _projectileType;
    public bool Homing;
    [SerializeField][Range(0, 15)] private int _radiusAMS, _projectileSpeed;
    [SerializeField][Range(0, 0.5f)] private float _projectileSize, _heat, _recoil;
    [SerializeField][Range(0, 1)] private float _damage, _fireDelay;
    [SerializeField][Range(0, 3000)] private float _fireRate, _laserRange;
    [Range(0, 15)] public int BurstSize; 
	[HideInInspector] public int DownTimer;
    [HideInInspector] public float LastBurst;
    private readonly float _spreadMult = 0.5f;
    private Vector3 _spreadVector;

    private float _spread, _updateTimer, _laserWidth;
    private readonly float _delay = 0.1f;

    [System.Serializable]
    public class WeaponModes
    {
        public string ModeName;
        public int FireMode;
    }
    public List<WeaponModes> weaponModes;

    private Transform _tubesContainer;
    private List<Transform> _tubes;

    [HideInInspector] public GameObject FirePoint, targetAMS;
    [HideInInspector] public UnitManager unitManager;
    [HideInInspector] public bool isFriend;
    private GameManager gameManager;
    private Collider colliderAMS;

    private LineRenderer lineRenderer;
    private bool laserOn, oneTime;
    private int shotsCount;

    void Awake()
    {
        FirePoint = transform.Find("FirePoint").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _updateTimer = Time.fixedTime + _delay;

        // Set AMS parameters 
        if (_projectileType == ProjectileType.AMS)
        {
            _damage = 0;
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
            colliderAMS.transform.localScale = _radiusAMS * Vector3.one;
        }
        else
        {
            _radiusAMS = 0;
        }

        if (_projectileType == ProjectileType.Missile)
        {
            // Count tubes to fire missile from each
            _tubesContainer = transform.Find("Tubes");
            for (int i = 0; i < _tubesContainer.childCount; i++)
            {
                _tubes.Add(_tubesContainer.GetChild(i));
            }
        }

        // Friend/Foe system for AMS to intercept
        if (_projectileType == ProjectileType.Missile && unitManager.transform.parent.parent.name == "PlayerSquad")
        {
            isFriend = true;
        }

        if (_projectileType == ProjectileType.Laser)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.startWidth = 0f;
            lineRenderer.endWidth = 0f;
            _projectileSpeed = 0;
            _recoil = 0;
        }
    }

    void FixedUpdate()
    {
        // Timer to slow update
        if (Time.fixedTime >= _updateTimer)
        {
            // Bullet spread 
            if (unitManager.transform.parent.name == "Player" && unitManager.Spread < 2
                && gameManager.inAction && _projectileType != ProjectileType.AMS)
            {
                unitManager.Spread += _spread / 50;
            }

            if (_spread > 0)
            {
                _spread -= Time.fixedDeltaTime * 2;
                unitManager.Spread = _spread;
            }
            else
            {
                _spread = 0;
                unitManager.Spread = _spread;
            }
            _updateTimer = Time.fixedTime + _delay;
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject target)
    {
        if (_projectileType != ProjectileType.Laser)
        {
            _spreadVector = new(
                Random.Range((-unitManager.MoveSpeed * _spreadMult) - _spread, (unitManager.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-unitManager.MoveSpeed * _spreadMult) - _spread / 2, (unitManager.MoveSpeed * _spreadMult) + _spread / 2),
                Random.Range((-unitManager.MoveSpeed * _spreadMult) - _spread, (unitManager.MoveSpeed * _spreadMult) + _spread));
        }
        else
        {
            //FireLaser coroutine to animate beam properly
            if (!oneTime)
            {
                ChangeShotsCount();
                this.Progress(gameManager.turnTime * 2, () =>
                {
                    FireLaser(target);
                });
                            
                oneTime = true;
                this.Wait(gameManager.turnTime, () =>
                {
                    oneTime = false;
                });
            }
        }

        if (_projectileType != ProjectileType.Laser && _projectileType != ProjectileType.AMS
            && !Homing)
        {
            FirePoint.transform.LookAt(target.transform.position + _spreadVector);
        }

        if (targetAMS != null)
        {
            FirePoint.transform.LookAt(targetAMS.transform.position + _spreadVector);
            if (targetAMS.transform.parent.localScale == Vector3.zero)
            {
                targetAMS = null;
            }
        }

        // Fire different projectiles
        if (Time.time > LastBurst + _fireDelay)
        {
            if (_projectileType == ProjectileType.Bullet)
            {
                StartCoroutine(FireBurstCoroutine(FirePoint, gameManager.bulletsPool));
                LastBurst = Time.time;
            }
            else if (_projectileType == ProjectileType.Missile && !Homing)
            {
                StartCoroutine(FireMissilesCoroutine(FirePoint, gameManager.missilesPool, target.transform.position));
                LastBurst = Time.time + gameManager.turnTime - _fireDelay; // fire burst once (increase delay)
            }
            else if (_projectileType == ProjectileType.Missile && Homing)
            {
                StartCoroutine(FireHMissilesCoroutine(FirePoint, gameManager.missilesPool, target));
                LastBurst = Time.time + gameManager.turnTime - _fireDelay; // fire burst once (increase delay)
            }
            else if (_projectileType == ProjectileType.AMS && targetAMS != null)
            {
                StartCoroutine(FireBurstCoroutine(FirePoint, gameManager.amsPool));
                LastBurst = Time.time;
            }
        }
    }

    private void FireLaser(GameObject target)
    {
        // Rotate laser when firing
        Vector3 direction = target.transform.position - FirePoint.transform.position;
        FirePoint.transform.rotation = Quaternion.RotateTowards(FirePoint.transform.rotation,
            Quaternion.LookRotation(direction + _spreadVector), Time.time * 0.01f);

        // Draw line
        lineRenderer.SetPosition(0, FirePoint.transform.position);
        if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward, out RaycastHit hit, _laserRange))
        {
            lineRenderer.SetPosition(1, hit.point);
        }
        lineRenderer.startWidth = _laserWidth;
        lineRenderer.endWidth = _laserWidth;

        // Laser animation
        if (laserOn)
        {
            DOTween.To(() => _laserWidth, x => _laserWidth = x, 0.03f, _fireDelay / 6);
            // Deal laser damage            
            if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward,
                out RaycastHit damageHit, _laserRange) && damageHit.collider.name == "Body")
            {
                damageHit.collider.GetComponent<UnitManager>().TakeDamage(_damage);
            }
        }
        else
        {
            DOTween.To(() => _laserWidth, x => _laserWidth = x, 0f, _fireDelay / 6);
        }

        // Stop laser at the end of turn
        if (!gameManager.inAction)
        {
            shotsCount = 0;
        }

        // Shoot laser  
        if (shotsCount > 0 && Time.time > LastBurst + _fireDelay)
        {
            _spreadVector = new(
                  Random.Range((-unitManager.MoveSpeed * _spreadMult) - _spread, (unitManager.MoveSpeed * _spreadMult) + _spread),
                  Random.Range((-unitManager.MoveSpeed * _spreadMult) - _spread, (unitManager.MoveSpeed * _spreadMult) + _spread),
                  Random.Range((-unitManager.MoveSpeed * _spreadMult) - _spread, (unitManager.MoveSpeed * _spreadMult) + _spread));

            laserOn = true;
            this.Wait(_fireDelay / 2, () =>
            {
                laserOn = false;
            });
            HeatRecoil();
            shotsCount -= 1;       
            LastBurst = Time.time;
        }        
    }

    // Coroutine for separate bursts
    private IEnumerator FireBurstCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, _projectileSize, _damage, _projectileSpeed);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for missiles
    private IEnumerator FireMissilesCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, Vector3 target)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            firePoint.transform.position = _tubes[i].position;
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, _projectileSize, _damage, _projectileSpeed, target, isFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for Homing missiles
    private IEnumerator FireHMissilesCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, GameObject target)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            firePoint.transform.position = _tubes[i].position;
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, _projectileSize, _damage, _projectileSpeed, target, isFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private void HeatRecoil()
    {
        if (unitManager.Heat < 1)
        {
            unitManager.Heat += _heat;
        }

        if (_spread < 1)
        {
            _spread += _recoil;
        }
    }

    // Set shots count for burst
    public void ChangeShotsCount()
    {
        shotsCount = BurstSize;        
    }
}
