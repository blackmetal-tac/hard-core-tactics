using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    [SerializeField][Range(0, 0.4f)] private float _projectileSize, _heat, _damage, _recoil;
    [SerializeField][Range(0, 3)] private float _fireDelay;
    [SerializeField][Range(0, 3000)] private float _fireRate, _laserRange;  
	[HideInInspector] public int BurstSize, DownTimer;
    [HideInInspector] public float LastBurst;
    private Vector3 _spreadVector, _laserPoint;
    private float _laserWidth;    

    [System.Serializable]
    public class WeaponModes
    {
        public string ModeName;
        public int FireMode;
    }
    public List<WeaponModes> WeaponModesP = new List<WeaponModes>();

    private Transform _tubesContainer;
    private List<Transform> _tubes = new List<Transform>();

    [HideInInspector] public GameObject FirePoint, TargetAMS;
    private CrosshairAMS _crosshairAMS;
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

        // Check if has 2 AMS' 
        _crosshairAMS = GameObject.Find("CrosshairAMS").GetComponent<CrosshairAMS>();
        if (!_crosshairAMS.Taken)
        {           
            _crosshairAMS.Taken = true;
        }
        else
        {
            _crosshairAMS = null;
        }

        if (_crosshairAMS == null)
        {
            _crosshairAMS = GameObject.Find("CrosshairAMS2").GetComponent<CrosshairAMS>();
            _crosshairAMS.Taken = true;
        }
        
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
        if (ProjectileTypeP == ProjectileType.AMS && TargetAMS != null 
            && TargetAMS.transform.parent.transform.localScale == Vector3.zero)
        {            
            TargetAMS = null;
            _crosshairAMS.transform.localScale = Vector3.zero;  
        }
    }

    // Set spawning projectile, fire point, delay between bursts, number of shots, fire rate
    public void FireBurst(UnitManager target)
    {
        if (!_oneTime)
        {
            LastBurstRandom(_fireDelay);
            _oneTime = true;
        }

        if (ProjectileTypeP == ProjectileType.Laser)
        {
            FireLaser(target);
        }  

        // AMS Crosshair position
        if (UnitManagerP.transform.parent.name == "Player" && ProjectileTypeP == ProjectileType.AMS && TargetAMS != null
            && BurstSize > 0)
        {  
            _crosshairAMS.transform.position = _camMain.WorldToScreenPoint(TargetAMS.transform.position);
            _crosshairAMS.transform.localScale = Vector3.one * 0.1f;
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
                StartCoroutine(FireBulletCoroutine(target));                
            }
            else if (ProjectileTypeP == ProjectileType.Missile && !Homing)
            {
                StartCoroutine(FireMissilesCoroutine(target));
            }
            else if (ProjectileTypeP == ProjectileType.Missile && Homing)
            {
                StartCoroutine(FireHMissilesCoroutine(target));                
            }
            else if (ProjectileTypeP == ProjectileType.AMS && TargetAMS != null)
            {
                StartCoroutine(FireAMSCoroutine());
            }
            LastBurstRandom(0);
        }
    }

    private void FireLaser(UnitManager target)
    {      
        // Move laser when firing 
        Vector3 direction = target.transform.position - FirePoint.transform.position;                
        _laserPoint = Vector3.MoveTowards(_laserPoint, direction + _spreadVector / 2, Time.deltaTime);
        FirePoint.transform.LookAt(_laserPoint); 
        
        // Set laser parameters
        _lineRenderer.SetPosition(0, FirePoint.transform.position);
        _lineRenderer.startWidth = _laserWidth;
        _lineRenderer.endWidth = _laserWidth;

        // Laser animation
        if (_laserOn)
        {          
            _laserWidth = Mathf.Lerp(_laserWidth, _damage * BurstSize, Time.deltaTime * _fireDelay / 0.3f);             

            // Deal laser damage            
            if (Physics.Raycast(FirePoint.transform.position, FirePoint.transform.forward,
                out RaycastHit hit, _laserRange, ~_ignoreLayers))
            {
                _lineRenderer.SetPosition(1, hit.point);
                if (hit.collider.name == "Body")
                {
                    hit.collider.GetComponent<UnitManager>().TakeDamage((_damage / 8) * _laserWidth);
                }
                else if (hit.collider.gameObject.layer == 17)
                {
                    hit.collider.GetComponent<Shield>().TakeDamage((_damage * 1.5f) * _laserWidth);
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
            _laserWidth = Mathf.Lerp(_laserWidth, 0f, Time.deltaTime * _fireDelay / 0.3f);            
        }

        // Shoot laser  
        if (Time.time > LastBurst + _fireDelay)
        {           
            _spreadVector = UnitManagerP.CalculateSpread();
            _laserPoint = target.transform.position - _spreadVector / 2;
            _laserOn = true;
            this.Wait(_fireDelay / 2, () =>
            {
                _laserOn = false;
            });  
            LastBurstRandom(0);
        }
    }

    private void LastBurstRandom(float delay)
    {
        LastBurst = Time.time - delay + Random.Range(0, 0.3f);
    }
    
    // Coroutine for separate bursts of bullets
    private IEnumerator FireBulletCoroutine(UnitManager target)
    {
        float shotDelay = 60 / _fireRate;
        for (int i = 0; i < BurstSize; i++)
        {
            _spreadVector = UnitManagerP.CalculateSpread();
            FirePoint.transform.LookAt(target.transform.position + _spreadVector);
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
            _spreadVector = UnitManagerP.CalculateSpread();
            if (TargetAMS != null)
            {
                FirePoint.transform.LookAt(TargetAMS.transform.position + _spreadVector / 2);
            }            
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
            _spreadVector = UnitManagerP.CalculateSpread();
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
            _spreadVector = UnitManagerP.CalculateSpread();
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

        if (UnitManagerP.Spread < 1)
        {
            UnitManagerP.Spread += _recoil;
        }        
    } 

    public float CalculateHeat()
    {
        float heat = 0;
        //float shots = 0;
        //float dmg = 0;
        if (ProjectileTypeP != ProjectileType.Laser)
        {
            heat = _heat * BurstSize * _gameManager.TurnTime / _fireDelay;
            //dmg = _damage * BurstSize * _gameManager.TurnTime / _fireDelay;
            //shots = BurstSize * _gameManager.TurnTime / _fireDelay;            
        }
        else
        {
            heat = _heat * BurstSize * _gameManager.TurnTime; 
            //dmg = _damage * BurstSize * _gameManager.TurnTime;
            //shots = _gameManager.TurnTime / _fireDelay;              
        }

        if (ProjectileTypeP == ProjectileType.AMS)
        {
            heat += _heat * 5 * _gameManager.TurnTime;
        } 
        //Debug.Log(transform.name + " shots " + shots);
        //Debug.Log(transform.name + " dmg " + dmg);
        //Debug.Log(transform.name + " heat " + heat);
        return heat;
    }

    public void EndMove()
    {
        if (_lineRenderer != null)
        {
            _laserOn = false; 
            this.Progress(1, () => {
                _lineRenderer.startWidth = _laserWidth;
                _lineRenderer.endWidth = _laserWidth; 
                _laserWidth = Mathf.Lerp(_laserWidth, 0f, Time.deltaTime * _fireDelay / 0.3f);         
            });
        }

        if (ProjectileTypeP == ProjectileType.AMS)
        {
            TargetAMS = null;
            _crosshairAMS.transform.localScale = Vector3.zero;
        }
        _oneTime = false;
    }
}
