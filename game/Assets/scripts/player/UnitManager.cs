using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
	[HideInInspector] public UnitManager Target;
    private GameObject _clickMarker;
    [HideInInspector] public Shield UnitShield;
	
    // Stats    
    [HideInInspector] public float HP, Heat, Cooling, MissileLockTimer;
	[Range(0, 1)] public float HeatTreshold;    
    [SerializeField][Range(0, 0.5f)] private float _shieldRegen;
    [SerializeField][Range(0, 1)] private float _heatCheckTime;
    [SerializeField][Range(0, 10)] private int _heatSafeRoll;
    [Range(0, 10)] public int WalkDistance;

	private GameManager _gameManager;
    private ShrinkBar _shrinkBar;
    private NavMeshAgent _navMeshAgent;
    private Vector3 _direction; // Rotate body to the enemy
    private readonly float _rotSpeed = 1f;
    private readonly float _spreadMult = 0.2f;

    [HideInInspector] public List<WPNManager> WeaponList;
    [HideInInspector] public int WeaponCount = 0, CoolingDownTimer, CoreDownTimer;
    private WeaponUI _weaponUI;

    [HideInInspector] public float MoveSpeed = 0.1f, Spread, ShrinkTimer, HeatModifier;
    [HideInInspector] public bool IsDead, AutoCooling, CoreSwitch; // Death trigger
    private bool _oneTime;
    private float _lastCheck;

    [System.Serializable]
    public class CoolingModes
    {
        public string ModeName;
        public float Cooling;
    }
    public List<CoolingModes> CoolingModesP; 
    
    [System.Serializable]
    public class CoreParameters
    {
        public int MoveBoost;        
    }
    [SerializeField] private CoreParameters _coreParameters;

    // ??? Set UnitManager for all weapons before Start
    void Awake()
    {
        /* Fill the list of all weapons on this unit (ORDER: rigth arm, left arm, rigth top,
            left top, rigth shoulder, left shoulder) */
        WeaponList.Add(transform.Find("Torso").Find("RightArm").Find("RightArmWPN").GetComponentInChildren<WPNManager>());
        WeaponList.Add(transform.Find("Torso").Find("LeftArm").Find("LeftArmWPN").GetComponentInChildren<WPNManager>());
        WeaponList.Add(transform.Find("Torso").Find("RightShoulderTopWPN").GetComponentInChildren<WPNManager>());
        WeaponList.Add(transform.Find("Torso").Find("LeftShoulderTopWPN").GetComponentInChildren<WPNManager>());
        WeaponList.Add(transform.Find("Torso").Find("RightArm").Find("RightShoulderWPN").GetComponentInChildren<WPNManager>());
        WeaponList.Add(transform.Find("Torso").Find("LeftArm").Find("LeftShoulderWPN").GetComponentInChildren<WPNManager>());

        // ??? assign unit manager for each weapon
        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {
                weapon.UnitManagerP = this;
                weapon.UnitID = transform.parent.name;
                WeaponCount += 1;
            }
        }
        UnitShield = transform.Find("Shield").GetComponentInChildren<Shield>();        
    }

    // Start is called before the first frame update
    void Start()
    { 
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _clickMarker = GameObject.Find("ClickMarker"); 
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        _navMeshAgent = transform.GetComponentInParent<NavMeshAgent>();
        _shrinkBar = GetComponentInChildren<ShrinkBar>();
        UnitShield.ShieldID = transform.parent.name;
        UnitShield.UnitManagerP = this;       
        Cooling = CoolingModesP[0].Cooling;
        Heat = 1;    

        // Load HP, Shield, Heat bars
        this.Progress(_gameManager.LoadTime, () => {
            if (UnitShield.HP < 1)
            {
                UnitShield.HP += Time.deltaTime * 0.6f; 
            }
            else
            {
                UnitShield.HP = 1;
            }

            if (HP < 1)
            {
                HP += Time.deltaTime * 0.6f;             
            }
            else
            {
                HP = 1;
            }

            if (Heat > 0)
            { 
                Heat -= Time.deltaTime * 0.6f;
            }
            else
            {
                Heat = 0;
            }
            _shrinkBar.UpdateHeat();
        });

        // Aim at the enemy ???
        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null && !weapon.Homing)
            {
                weapon.FirePoint.transform.LookAt(Target.transform.position);
            }
        }
    }

    void Update()
    {
        _shrinkBar.UpdateShield();
        _shrinkBar.UpdateHealth();

        if (Spread > 0)
        {
            Spread -= Time.deltaTime;
        }                  
      
        if (MissileLockTimer > 0)
        {
            MissileLockTimer -= Time.deltaTime;
        } 

        // Refresh after unit switching
        if (IsDead && !_oneTime || Target.IsDead && !_oneTime)
        {
            EndMove();
            _oneTime = true;
        }          
    }

    // Do actions in Update
    public void StartAction()
    {
        if (!IsDead)
        {
            // For tests
            // if (!Target.IsDead && transform.parent.name == "Player")
            if (!Target.IsDead)
            {
                StartShoot();
            }            

            // Shield regeneration
            UnitShield.Regenerate();
            
            // Heat dissipation
            if (Heat > 0)
            {
                Heat -= Time.deltaTime * Cooling;                
                _shrinkBar.UpdateHeat();

                // Roll Heat penalty
                if (Heat > HeatTreshold && Time.time > _lastCheck + _heatCheckTime) 
                {
                    OverheatRoll();
                    _lastCheck = Time.time;
                }
                else if (Heat >= 1f && Time.time > _lastCheck + 0.3f)
                {
                    Overheat();
                    _lastCheck = Time.time;
                }
            }
        }
    }

    private void StartShoot()
    {
        // Rotate body to Target
        _direction = Target.transform.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, Quaternion.LookRotation(_direction), Time.time * _rotSpeed);

        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {
                weapon.FireBurst(Target);
            }
        }
        _weaponUI.CoreButtonP.UpdateStatus();
    }

    // Set move position to maximum move distance (speed)
    public void SetDestination(Vector3 movePoint, NavMeshAgent navAgent)
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.speed = 0;
        
        NavMesh.CalculatePath(transform.position, movePoint, NavMesh.AllAreas, path);
        float pathLenght = GetPathLength(path);        
        for (int i = 0; i < path.corners.Length - 1; i++)
        {            
           
            if (pathLenght <= WalkDistance)
            {
                navAgent.SetDestination(movePoint);
                MoveSpeed = pathLenght / _gameManager.TurnTime;                
            }
            else
            {
                Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * WalkDistance);
                navAgent.SetDestination(finalPoint);
                MoveSpeed = WalkDistance / _gameManager.TurnTime;                
                break;
            }
        }
    }

    public float GetPathLength(NavMeshPath path)
    {
        float lenght = 0;       
        for (int i = 1; i < path.corners.Length; ++i)
        {
            lenght += Vector3.Distance(path.corners[i - 1], path.corners[i]);
        }
        return lenght;
    }

    // Take damage
    public void TakeDamage(float damage)
    {
        // Reset HP bar damage animation
        ShrinkTimer = 0.5f;
        if (HP > 0)
        {
            HP -= damage;            
        }

        // Death
        if (HP <= 0)
        {
            IsDead = true;            
            _navMeshAgent.enabled = false;
            GetComponent<Collider>().enabled = false;
            transform.localScale = Vector3.zero;
        }
    }

    // Roll chance for overheat
    private void OverheatRoll()
    {
        int rollHeat = Random.Range(1,10);
        if (_heatSafeRoll < rollHeat)
        {
            Overheat();
        }
    }

    // Roll a weapon to overheat
    private void Overheat()
    {
        int wpnIndex = Random.Range(0, WeaponCount + 1);
        if (wpnIndex < WeaponCount && WeaponList[wpnIndex] != null && WeaponList[wpnIndex].DownTimer <= 0)
        {
            WeaponList[wpnIndex].DownTimer = 3;
            WeaponList[wpnIndex].BurstSize = WeaponList[wpnIndex].WeaponModesP[0].FireMode;            

            if (transform.parent.name == "Player")
            {
                _weaponUI.WeaponDown(wpnIndex, WeaponList[wpnIndex].DownTimer);
            }
        } 
        else if (wpnIndex == WeaponCount && UnitShield.DownTimer <= 0)
        {
            DisableShield();
        }
    }

    // Update timers for overheated weapon
    public void UpdateOverheatTimer()
    {
        for (int i = 0; i < WeaponList.Count; i++)
        {
            if (WeaponList[i] != null)
            {
                WeaponList[i].DownTimer -= 1;

                if (transform.parent.name == "Player" && WeaponList[i].DownTimer > 0)
                {
                    _weaponUI.UpdateStatus(i, WeaponList[i].DownTimer); // ???
                }
                else if (transform.parent.name == "Player" && WeaponList[i].DownTimer <= 0)
                {
                    _weaponUI.WeaponUp(i);
                }
            }
        }

        UnitShield.DownTimer -= 1;
        if (transform.parent.name == "Player" && UnitShield.DownTimer > 0)
        {
            _weaponUI.UpdateStatus(6, UnitShield.DownTimer); // ???
        }
        else if (transform.parent.name == "Player" && UnitShield.DownTimer <= 0)
        {
            _weaponUI.WeaponUp(6);
        }

        CoolingDownTimer -= 1;
        if (transform.parent.name == "Player" && CoolingDownTimer > 0)
        {
            _weaponUI.UpdateStatus(7, CoolingDownTimer); // ???
        }
        else if (transform.parent.name == "Player" && CoolingDownTimer <= 0)
        {
            _weaponUI.WeaponUp(7);
        }
        CoolingOverdrive();

        CoreDownTimer -= 1;
        if (CoreDownTimer == 3) // ???
        {
            CoreSwitch = !CoreSwitch;
            WalkDistance -= _coreParameters.MoveBoost; 
        }
        _weaponUI.CoreButtonP.UpdateStatus();
    }

    public void CoolingOverdrive()
    {
        if (Cooling == CoolingModesP[1].Cooling && CoolingDownTimer <= 0 )
        {
            CoolingDownTimer = 5; 
            if (transform.parent.name == "Player")
            {                
                _weaponUI.WeaponDown(7, CoolingDownTimer);
            }
        }
        else if (CoolingDownTimer <= 3)
        {
            Cooling = CoolingModesP[0].Cooling;
        }
    }    

    public void CoreOverdrive()
    {        
        CoreSwitch = !CoreSwitch;        
        if (CoreSwitch && CoreDownTimer <= 0)
        {
            CoreDownTimer = 5;
            WalkDistance *= _coreParameters.MoveBoost;
        }
        else
        {
            CoreDownTimer = 0;
            WalkDistance /= _coreParameters.MoveBoost;      

            if (transform.parent.name == "Player")
            {                
                SetDestination(_clickMarker.transform.position, _navMeshAgent);
            }                
        }        
    }

    public void DisableShield()
    {
        UnitShield.DownTimer = 3;
        UnitShield.ChangeMode(UnitShield.shieldModes[0]); 
        UnitShield.TurnOnOff();

        if (transform.parent.name == "Player")
        {                
            _weaponUI.WeaponDown(6, UnitShield.DownTimer);
        }
    }

    public Vector3 CalculateSpread()
    {
        Vector3 spread = new(
                Random.Range((-MoveSpeed * _spreadMult) - Spread, (MoveSpeed * _spreadMult) + Spread),
                Random.Range((-MoveSpeed * _spreadMult) - Spread / 2, (MoveSpeed * _spreadMult) + Spread / 2),
                Random.Range((-MoveSpeed * _spreadMult) - Spread, (MoveSpeed * _spreadMult) + Spread));
        
        return spread;
    }
    
    public void EndMove()
    {
        UpdateOverheatTimer();
        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {
                weapon.EndMove();
            }
        }
    }
}
