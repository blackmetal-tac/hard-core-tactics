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
    [SerializeField] private bool _burstLaser;
    [SerializeField][Range(0, 15)] private int _radiusAMS, _projectileSpeed;
    [SerializeField][Range(0, 0.5f)] private float _projectileSize, _heat, _recoil, _damage;
    [SerializeField][Range(0, 1)] private float _fireDelay;
    [SerializeField][Range(0, 3000)] private float _fireRate, _laserRange;  
	[HideInInspector] public int BurstSize, DownTimer;
    [HideInInspector] public float LastBurst;
    private readonly float _spreadMult = 0.5f;
    private Vector3 _spreadVector, _laserPoint;
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

    [HideInInspector] public GameObject FirePoint, TargetAMS;
    [HideInInspector] public UnitManager UnitManager;
    private bool _isFriend;
    private GameManager _gameManager;
    private Collider _colliderAMS;

    private LineRenderer _lineRenderer;
    private bool _laserOn, _oneTime;
    private int _shotsCount;
    [HideInInspector] public string UnitID;

    void Awake()
    {
        FirePoint = transform.Find("FirePoint").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _updateTimer = Time.time + _delay;

        // Set AMS parameters 
        if (_projectileType == ProjectileType.AMS)
        {
            _damage = 0;
            _colliderAMS = GetComponentInChildren<Collider>();
            if (UnitManager.transform.parent.parent.name == "PlayerSquad")
            {
                _colliderAMS.gameObject.layer = 14;
            }
            else
            {
                _colliderAMS.gameObject.layer = 15;
            }
            _colliderAMS.enabled = true;
            _colliderAMS.transform.localScale = _radiusAMS * Vector3.one;
        }
        else
        {
            _radiusAMS = 0;
        }

        if (_projectileType == ProjectileType.Missile)
        {
            // Count tubes to fire missile from each
            _tubesContainer = transform.Find("Tubes");       
            _tubes = new List<Transform>();     
            for (int i = 0; i < _tubesContainer.childCount; i++)
            {
                _tubes.Add(_tubesContainer.GetChild(i));
            }
        }

        // Friend/Foe system for AMS to intercept
        if (UnitManager.transform.parent.parent.name == "PlayerSquad")
        {
            _isFriend = true;
        }

        if (_projectileType == ProjectileType.Laser)
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _lineRenderer.startWidth = 0f;
            _lineRenderer.endWidth = 0f;
            _projectileSpeed = 0;
            _recoil = 0;            
        }
    }

    void Update()
    {
        // Timer to slow update
        if (Time.time >= _updateTimer)
        {
            // Bullet spread for UI crosshair 
            if (UnitManager.transform.parent.name == "Player" && UnitManager.Spread < 2
                && _gameManager.InAction && _projectileType != ProjectileType.AMS)
            {
                UnitManager.Spread += _spread / 50;
            }

            if (_spread > 0)
            {
                _spread -= Time.deltaTime * 2;
                UnitManager.Spread = _spread;
            }
            else
            {
                _spread = 0;
                UnitManager.Spread = _spread;
            }
            _updateTimer = Time.time + _delay;
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(GameObject target)
    {
        if (_projectileType != ProjectileType.Laser)
        {
            _spreadVector = new(
                Random.Range((-UnitManager.MoveSpeed * _spreadMult) - _spread, (UnitManager.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-UnitManager.MoveSpeed * _spreadMult) - _spread / 2, (UnitManager.MoveSpeed * _spreadMult) + _spread / 2),
                Random.Range((-UnitManager.MoveSpeed * _spreadMult) - _spread, (UnitManager.MoveSpeed * _spreadMult) + _spread));
        }
        else
        {
            //FireLaser coroutine to animate beam properly
            if (!_oneTime)
            {
                ChangeShotsCount();                
                this.Progress(_gameManager.TurnTime * 2, () =>
                {
                    FireLaser(target);
                });
                            
                _oneTime = true;
                this.Wait(_gameManager.TurnTime, () =>
                {
                    _oneTime = false;
                });
            }
        }

        if (_projectileType != ProjectileType.Laser || _projectileType != ProjectileType.AMS
            || !Homing)
        {
            FirePoint.transform.LookAt(target.transform.position + _spreadVector);
        }

        if (_projectileType == ProjectileType.AMS && TargetAMS != null)
        {
            FirePoint.transform.LookAt(TargetAMS.transform.position + _spreadVector);
            if (TargetAMS.transform.parent.localScale == Vector3.zero)
            {
                TargetAMS = null;
            }
        }

        // Fire different projectiles
        if (Time.time > LastBurst + _fireDelay)
        {
            if (_projectileType == ProjectileType.Bullet)
            {
                StartCoroutine(FireBulletCoroutine(FirePoint, _gameManager.BulletsPool, UnitID));
                LastBurst = Time.time;
            }
            else if (_projectileType == ProjectileType.Missile && !Homing)
            {
                StartCoroutine(FireMissilesCoroutine(FirePoint, _gameManager.MissilesPool, target.transform.position));
                LastBurst = Time.time + _gameManager.TurnTime - _fireDelay; // fire burst once (increase delay)
            }
            else if (_projectileType == ProjectileType.Missile && Homing)
            {
                StartCoroutine(FireHMissilesCoroutine(FirePoint, _gameManager.MissilesPool, target));
                LastBurst = Time.time + _gameManager.TurnTime - _fireDelay; // fire burst once (increase delay)
            }
            else if (_projectileType == ProjectileType.AMS && TargetAMS != null)
            {
                StartCoroutine(FireAMSCoroutine(FirePoint, _gameManager.AmsPool));
                LastBurst = Time.time;
            }
        }
    }

    private void FireLaser(GameObject target)
    {      
        // Move laser when firing 
        Vector3 direction = target.transform.position - FirePoint.transform.position;   
        _laserPoint = Vector3.MoveTowards(_laserPoint, direction + _spreadVector, Time.deltaTime * 2);
        FirePoint.transform.LookAt(_laserPoint); 
        
        // Draw line forward
        _lineRenderer.SetPosition(0, FirePoint.transform.position);
        if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward, out RaycastHit hit, _laserRange))
        {
            _lineRenderer.SetPosition(1, hit.point);            
        }
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;

        // Laser animation
        if (_laserOn)
        {
            DOTween.To(() => _laserWidth, x => _laserWidth = x, _damage * BurstSize, _fireDelay / 6);

            // Deal laser damage            
            if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward,
                out RaycastHit damageHit, _laserRange))
            {
                if (damageHit.collider.name == "Body")
                {
                    damageHit.collider.GetComponent<UnitManager>().TakeDamage(_damage * _laserWidth);
                }
                else if (damageHit.collider.gameObject.layer == 17)
                {
                    damageHit.collider.GetComponent<Shield>().TakeDamage(_damage * _laserWidth * 2);                    
                }
                else if (_isFriend && damageHit.collider.gameObject.layer == 13 // Detonate foe's missiles
                    || !_isFriend && damageHit.collider.gameObject.layer == 12)
                {
                    damageHit.collider.GetComponent<Missile>().Explode();                                       
                }
            }

            if (UnitManager.Heat < 1)
            {
                UnitManager.Heat += _heat * _laserWidth;
            }
        }
        else
        {
            DOTween.To(() => _laserWidth, x => _laserWidth = x, 0f, _fireDelay / 6);
        }

        // Stop laser at the end of turn
        if (!_gameManager.InAction)
        {
            _shotsCount = 0;            
        }

        // Shoot laser  
        if (_shotsCount > 0 && Time.time > LastBurst + _fireDelay)
        {
            _laserPoint = target.transform.position;
            _spreadVector = new(
                Random.Range((-UnitManager.MoveSpeed * _spreadMult) - _spread, (UnitManager.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-UnitManager.MoveSpeed * _spreadMult) - _spread, (UnitManager.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-UnitManager.MoveSpeed * _spreadMult) - _spread, (UnitManager.MoveSpeed * _spreadMult) + _spread));
           
            _laserOn = true;
            this.Wait(_fireDelay / 2, () =>
            {
                _laserOn = false;
            });            
            _shotsCount -= 1;       
            LastBurst = Time.time;
        }        
    }
    
    // Coroutine for separate bursts of bullets
    private IEnumerator FireBulletCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool, string unitID)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, _projectileSize, _damage, _projectileSpeed, unitID);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for separate bursts of AMS
    private IEnumerator FireAMSCoroutine(GameObject firePoint, ObjectPool<PoolObject> objectPool)
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
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, _projectileSize, _damage, _projectileSpeed, target, _isFriend);
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
            objectPool.PullGameObject(firePoint.transform.position, firePoint.transform.rotation, _projectileSize, _damage, _projectileSpeed, target, _isFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private void HeatRecoil()
    {
        if (UnitManager.Heat < 1)
        {
            UnitManager.Heat += _heat;
        }

        if (_spread < 1)
        {
            _spread += _recoil;
        }
    }

    // Set shots count for burst
    public void ChangeShotsCount()
    {
        if (_burstLaser)
        {
            _shotsCount = BurstSize;
        }
        else
        {
            _shotsCount = 1;
        }              
    }
}
