using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
	[HideInInspector] public GameObject Target;
    [HideInInspector] public Shield UnitShield;
	
    // Stats    
    [HideInInspector] public float HP, Heat, Cooling;
	[Range(0, 1)] public float HeatTreshold;
    [SerializeField][Range(0, 0.5f)] private float _shieldRegen;
    [SerializeField][Range(0, 1)] private float _heatCheckTime;
    [SerializeField][Range(0, 10)] private int _heatSafeRoll;
    [SerializeField][Range(0, 10)] private int _walkDistance;

	private GameManager _gameManager;
    private ShrinkBar _shrinkBar;
    private NavMeshAgent _navMeshAgent;
    private Vector3 _direction; // Rotate body to the enemy
    private readonly float _rotSpeed = 1f;

    [HideInInspector] public List<WPNManager> WeaponList;
    [HideInInspector] public int WeaponCount = 0, CoolingDownTimer;
    private WeaponUI _weaponUI;

    [HideInInspector] public float MoveSpeed = 0.1f, Spread, ShrinkTimer;
    [HideInInspector] public bool IsDead, AutoCooling; // Death trigger
    private float _lastCheck;

    [System.Serializable]
    public class CoolingModes
    {
        public string ModeName;
        public float Cooling;
    }
    public List<CoolingModes> coolingModes;

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
                weapon.UnitManager = this;
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
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        _navMeshAgent = transform.GetComponentInParent<NavMeshAgent>();
        _shrinkBar = GetComponentInChildren<ShrinkBar>();
        UnitShield.ShieldID = transform.parent.name;
        Heat = 1;
        Cooling = coolingModes[0].Cooling;

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

        // Load Heat
        this.Progress(_gameManager.LoadTime, () => {
            if (UnitShield.HP < 1)
            {
                UnitShield.HP += Time.deltaTime * 0.6f;
            }

            if (HP < 1)
            {
                HP += Time.deltaTime * 0.6f;
            }
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

    void FixedUpdate()
    {
        _shrinkBar.UpdateShield();
        _shrinkBar.UpdateHealth();
    }

    // Do actions in Update
    public void StartAction()
    {
        if (!IsDead)
        {
            StartShoot();

            // Shield regeneration
            if (!IsDead)
            {            
                UnitShield.Regenerate();            
            }

            // Heat dissipation
            if (Heat > 0 && !IsDead)
            {
                Heat -= Time.deltaTime * (Cooling - UnitShield.Heat);                
                _shrinkBar.UpdateHeat();

                if (Heat > HeatTreshold && Time.time > _lastCheck + _heatCheckTime) // Roll Heat penalty
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
    }

    // Set move position and maximum move distance (speed)
    public void SetDestination(Vector3 movePoint, NavMeshAgent navAgent)
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.speed = 0;

        NavMesh.CalculatePath(transform.position, movePoint, NavMesh.AllAreas, path);
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;
            if (segmentDistance <= _walkDistance)
            {
                navAgent.SetDestination(movePoint);
                MoveSpeed = segmentDistance / _gameManager.TurnTime;
                MoveSpeed = Mathf.Round(100 * MoveSpeed) / 100;
            }
            else
            {
                Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * _walkDistance);
                NavMesh.CalculatePath(transform.position, finalPoint, NavMesh.AllAreas, path);
                navAgent.SetPath(path);
                MoveSpeed = _walkDistance / _gameManager.TurnTime;
                break;
            }
        }
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
            WeaponList[wpnIndex].BurstSize = WeaponList[wpnIndex].weaponModes[0].FireMode;
            WeaponList[wpnIndex].ChangeShotsCount();

            if (transform.parent.name == "Player")
            {
                _weaponUI.WeaponDown(wpnIndex, WeaponList[wpnIndex].DownTimer);
            }
        } 
        else if (wpnIndex == WeaponCount && UnitShield.DownTimer <= 0)
        {
            UnitShield.DownTimer = 3;
            UnitShield.ChangeMode(UnitShield.shieldModes[0]); 
            UnitShield.TurnOnOff();

            if (transform.parent.name == "Player")
            {                
                _weaponUI.WeaponDown(wpnIndex, UnitShield.DownTimer);
            }
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
    }

    public void CoolingOverdrive()
    {
        if (Cooling == coolingModes[1].Cooling && CoolingDownTimer <= 0 && !AutoCooling)
        {
            CoolingDownTimer = 5; 
            if (transform.parent.name == "Player")
            {                
                _weaponUI.WeaponDown(7, CoolingDownTimer);
            }
        }
    }    
}
