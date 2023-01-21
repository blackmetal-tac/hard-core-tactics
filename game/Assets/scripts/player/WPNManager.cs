using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class WPNManager : MonoBehaviour
{
    // Weapon stats
    public enum ProjectileType { Bullet, Missile, Laser, AMS }
    public ProjectileType ProjectileTypeP;
    private Camera _camMain;
    [SerializeField] private LayerMask _ignoreLayers;
    public bool Homing;
    [HideInInspector] public bool IsFriend;
    [SerializeField][Range(0, 20)] private int _radiusAMS, _projectileSpeed;
    [SerializeField][Range(0, 0.3f)] private float _projectileSize, _heat, _damage;
    [SerializeField][Range(0, 2)] private float _recoil, _fireDelay;
    [SerializeField][Range(0, 3000)] private float _fireRate, _laserRange;  
	[HideInInspector] public int BurstSize, DownTimer;
    [HideInInspector] public float LastBurst;
    private readonly float _spreadMult = 0.2f;
    private Vector3 _spreadVector, _laserPoint;
    private float _spread, _updateTimer, _laserWidth;
    private readonly float _delay = 0.1f;
    float totalShots;

    [System.Serializable]
    public class WeaponModes
    {
        public string ModeName;
        public int FireMode;
    }
    public List<WeaponModes> WeaponModesP;

    private Transform _tubesContainer;
    private List<Transform> _tubes;

    [HideInInspector] public GameObject FirePoint, TargetAMS;
    private GameObject _crosshairAMS;
    [HideInInspector] public UnitManager UnitManagerP;    
    private GameManager _gameManager;
    private Collider _colliderAMS;

    private LineRenderer _lineRenderer;
    private bool _laserOn, _oneTime;
    [HideInInspector] public string UnitID;

    void Awake()
    {
        FirePoint = transform.Find("FirePoint").gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();        
        _crosshairAMS = GameObject.Find("CrosshairAMS");
        _updateTimer = Time.time + _delay;
        _camMain = Camera.main;

        // Friend/Foe system for AMS to intercept
        if (UnitManagerP.transform.parent.parent.name == "PlayerSquad")
        {
            IsFriend = true;            
        }

        // Set AMS parameters 
        if (ProjectileTypeP == ProjectileType.AMS)
        {
            _damage = 0;
            _colliderAMS = GetComponentInChildren<Collider>();
            if (IsFriend)
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

        if (ProjectileTypeP == ProjectileType.Missile)
        {
            // Count tubes to fire missile from each
            _tubesContainer = transform.Find("Tubes");       
            _tubes = new List<Transform>();     
            for (int i = 0; i < _tubesContainer.childCount; i++)
            {
                _tubes.Add(_tubesContainer.GetChild(i));
            }
        }

        if (ProjectileTypeP == ProjectileType.Laser)
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
            if (UnitManagerP.transform.parent.name == "Player" && UnitManagerP.Spread < 2
                && _gameManager.InAction && ProjectileTypeP != ProjectileType.AMS)
            {
                UnitManagerP.Spread += _spread / 50;
            }

            if (_spread > 0)
            {
                _spread -= Time.deltaTime * 2;
                UnitManagerP.Spread = _spread;
            }
            else
            {
                _spread = 0;
                UnitManagerP.Spread = _spread;
            }
            _updateTimer = Time.time + _delay;
        }

        if (_lineRenderer != null && !_gameManager.InAction || _lineRenderer != null && UnitManagerP.IsDead
            || _lineRenderer != null && UnitManagerP.Target.IsDead || _lineRenderer != null && BurstSize == 0)
        {
            _laserOn = false;
            _lineRenderer.startWidth = _laserWidth;
            _lineRenderer.endWidth = _laserWidth;
            DOTween.To(() => _laserWidth, x => _laserWidth = x, 0f, _fireDelay / 6);
        }

        if (ProjectileTypeP == ProjectileType.AMS && TargetAMS != null 
            && TargetAMS.transform.parent.transform.localScale == Vector3.zero)
        {            
            TargetAMS = null;
        }

        if (UnitManagerP.transform.parent.name == "Player" && ProjectileTypeP == ProjectileType.AMS && TargetAMS == null) 
        {
            _crosshairAMS.transform.localScale = Vector3.zero;                     
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(UnitManager target)
    {
        if (ProjectileTypeP != ProjectileType.Laser && BurstSize > 0)
        {
            _spreadVector = new(
                Random.Range((-UnitManagerP.MoveSpeed * _spreadMult) - _spread, (UnitManagerP.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-UnitManagerP.MoveSpeed * _spreadMult) - _spread / 2, (UnitManagerP.MoveSpeed * _spreadMult) + _spread / 2),
                Random.Range((-UnitManagerP.MoveSpeed * _spreadMult) - _spread, (UnitManagerP.MoveSpeed * _spreadMult) + _spread));
        }
        else if (BurstSize > 0)
        {
            FireLaser(target);
        }

        if (ProjectileTypeP == ProjectileType.Bullet && BurstSize > 0
            || ProjectileTypeP == ProjectileType.Missile && BurstSize > 0 && !Homing)
        {
            FirePoint.transform.LookAt(target.transform.position + _spreadVector);                       
        }

        if (ProjectileTypeP == ProjectileType.AMS && TargetAMS != null)
        {
            FirePoint.transform.LookAt(TargetAMS.transform.position + _spreadVector);            

            if (UnitManagerP.transform.parent.name == "Player")
            {
                // AMS Crosshair position
                _crosshairAMS.transform.position = _camMain.WorldToScreenPoint(TargetAMS.transform.position);
                _crosshairAMS.transform.localScale = Vector3.one * 0.1f;
            }
        }

        // AMS heat generation
        if (ProjectileTypeP == ProjectileType.AMS && BurstSize > 0)
        {
            UnitManagerP.Heat += Time.deltaTime * _heat * 5;
        }

        // Fire different projectiles
        if (Time.time > LastBurst + _fireDelay && BurstSize > 0)
        {
            if (ProjectileTypeP == ProjectileType.Bullet)
            {
                StartCoroutine(FireBulletCoroutine());
                LastBurst = Time.time;
            }
            else if (ProjectileTypeP == ProjectileType.Missile && !Homing)
            {
                StartCoroutine(FireMissilesCoroutine(target));                
                LastBurst = Time.time;
            }
            else if (ProjectileTypeP == ProjectileType.Missile && Homing)
            {
                StartCoroutine(FireHMissilesCoroutine(target));
                LastBurst = Time.time;
            }
            else if (ProjectileTypeP == ProjectileType.AMS && TargetAMS != null)
            {
                StartCoroutine(FireAMSCoroutine());                
                LastBurst = Time.time;
            }
        }
    }

    private void FireLaser(UnitManager target)
    {      
        // Move laser when firing 
        Vector3 direction = target.transform.position - FirePoint.transform.position;   
        _laserPoint = Vector3.MoveTowards(_laserPoint, direction + _spreadVector, Time.deltaTime * 1);
        FirePoint.transform.LookAt(_laserPoint); 
        
        // Set laser parameters
        _lineRenderer.SetPosition(0, FirePoint.transform.position);
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;

        // Laser animation
        if (_laserOn)
        {            
            DOTween.To(() => _laserWidth, x => _laserWidth = x, _damage * BurstSize, _fireDelay / 6);

            // Deal laser damage            
            if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward,
                out RaycastHit hit, _laserRange, ~_ignoreLayers))
            {
                _lineRenderer.SetPosition(1, hit.point);
                if (hit.collider.name == "Body")
                {
                    hit.collider.GetComponent<UnitManager>().TakeDamage((_damage / 4) * _laserWidth);
                }
                else if (hit.collider.gameObject.layer == 17)
                {
                    hit.collider.GetComponent<Shield>().TakeDamage((_damage * 2) * _laserWidth);
                }
                else if (IsFriend && hit.collider.gameObject.layer == 13 // Detonate foe's missiles
                    || !IsFriend && hit.collider.gameObject.layer == 12)
                {
                    hit.collider.GetComponent<Missile>().Explode();                                       
                }
            }

            if (UnitManagerP.Heat < 1)
            {
                UnitManagerP.Heat += _heat * _laserWidth;
            }
        }
        else
        {
            DOTween.To(() => _laserWidth, x => _laserWidth = x, 0f, _fireDelay / 6);
        }

        // Shoot laser  
        if (Time.time > LastBurst + _fireDelay)
        {
            _laserPoint = target.transform.position;
            _spreadVector = new(
                Random.Range((-UnitManagerP.MoveSpeed * _spreadMult) - _spread, (UnitManagerP.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-UnitManagerP.MoveSpeed * _spreadMult) - _spread, (UnitManagerP.MoveSpeed * _spreadMult) + _spread),
                Random.Range((-UnitManagerP.MoveSpeed * _spreadMult) - _spread, (UnitManagerP.MoveSpeed * _spreadMult) + _spread));
           
            _laserOn = true;
            this.Wait(_fireDelay / 2, () =>
            {
                _laserOn = false;
            });  
            LastBurst = Time.time;
        }
    }
    
    // Coroutine for separate bursts of bullets
    private IEnumerator FireBulletCoroutine()
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            _gameManager.BulletsPool.PullGameObject(FirePoint.transform, _projectileSize, _damage, _projectileSpeed, UnitID);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }        
    }

    // Coroutine for separate bursts of AMS
    private IEnumerator FireAMSCoroutine()
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        { 
            _gameManager.AmsPool.PullGameObject(FirePoint.transform, _projectileSize, _projectileSpeed);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for missiles
    private IEnumerator FireMissilesCoroutine(UnitManager target)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            FirePoint.transform.position = _tubes[i].position;
            _gameManager.MissilesPool.PullGameObject(FirePoint.transform, _spreadVector, _projectileSize, _damage, 
                _projectileSpeed, target, IsFriend);
            HeatRecoil();
            yield return new WaitForSeconds(shotDelay);
        }
    }

    // Coroutine for Homing missiles
    private IEnumerator FireHMissilesCoroutine(UnitManager target)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            FirePoint.transform.position = _tubes[i].position;
            _gameManager.MissilesPool.PullGameObject(FirePoint.transform, _projectileSize, _damage, 
                _projectileSpeed, target, IsFriend);
            HeatRecoil();            
            yield return new WaitForSeconds(shotDelay);
        }
    }

    private void HeatRecoil()
    {
        if (UnitManagerP.Heat < 1)
        {
            UnitManagerP.Heat += _heat;
        }

        if (_spread < 1)
        {
            _spread += _recoil;
        }
    } 
}
