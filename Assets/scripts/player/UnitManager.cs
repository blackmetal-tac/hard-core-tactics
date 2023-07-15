using UnityEngine;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    // Units classes 
    // Assault - balanced
    // Scout - fast and nimble / Speed * 1.25 / no aim penalty while moving
    // Heavy - slow and armored / Speed * 0.75 / takeDamage * 0.75
    private enum UnitClass { Assault, Scout, Heavy }
    [SerializeField] private UnitClass _unitClass;
	[HideInInspector] public UnitManager Target;
    private GameObject _clickMarker, _legs, _torso, _leftArm, _rightArm, _shield;
    private AIController _unitController;
    private LegsController _legsController;
    private ArmsController _leftArmCont, _rightArmCont;
    [HideInInspector] public Shield UnitShield;
	
    // Stats    
    [HideInInspector] public float HP, Heat, Cooling, HeatCalc, MissileLockTimer;
	[Range(0, 1)] public float HeatTreshold;
    [SerializeField][Range(0, 1)] private float _heatCheckTime;
    [SerializeField][Range(0, 10)] private int _heatSafeRoll;
    [Range(0, 20)] public int WalkDistance;

	private GameManager _gameManager;
    [HideInInspector] public ShrinkBar ShrinkBar;
    private Vector3 _direction; // Rotate body to the enemy
    private readonly float _rotSpeed = 0.2f;
    [HideInInspector] public float SpreadMult = 0.1f;

    [HideInInspector] public List<WPNManager> WeaponList = new List<WPNManager>();
    [HideInInspector] public int WeaponCount = 0, CoolingDownTimer, CoreDownTimer;
    private WeaponUI _weaponUI;

    [HideInInspector] public float MoveSpeed = 0.1f, Spread, ShrinkTimer, HeatModifier;
    [HideInInspector] public bool IsDead, AutoCooling, CoolOverdrive, CoreSwitch;
    private bool _oneTime;
    private float _lastCheck, _armor = 1;

    [System.Serializable]
    public class CoolingModes
    {
        public string ModeName;
        public float Cooling;
    }
    public List<CoolingModes> CoolingModesP = new List<CoolingModes>(); 

    // Lists of parts and weapons for instanses ???
    [SerializeField] private List<GameObject> _mechParts = new List<GameObject>(), _mechWeapons = new List<GameObject>();
    private List<GameObject> _mountedWeapons = new List<GameObject>();
    
    [System.Serializable]
    public class CoreParameters
    {
        public int MoveBoost;        
    }
    [SerializeField] private CoreParameters _coreParameters;

    // ??? Set UnitManager for all weapons before Start
    void Awake()
    { 
        // Spawn mech components (0 - legs 1 - torso 2-3 - arms 4 - shield)        
        _legs = GameObject.Instantiate(_mechParts[0], transform.position, Quaternion.Euler(-90, 0, 0), transform); 
        _legsController = _legs.GetComponent<LegsController>();

        Transform mountTransform = _legs.GetComponentInChildren<Hips>().transform;
        _torso = GameObject.Instantiate(_mechParts[1], mountTransform.position, Quaternion.Euler(-90, 0, 0), mountTransform);      
        _torso.transform.localPosition = new Vector3(0, 0, 0.25f);

        _shield = GameObject.Instantiate(_mechParts[4], mountTransform.position, Quaternion.Euler(0, 0, 0), mountTransform);
        UnitShield = _shield.GetComponent<Shield>(); 

        mountTransform = _torso.GetComponentInChildren<LeftShoulder>().transform;
        _leftArm = GameObject.Instantiate(_mechParts[2], mountTransform.position, Quaternion.Euler(-90, 0, 0), mountTransform);
        _leftArmCont = _leftArm.GetComponent<ArmsController>();
        _leftArm.transform.localPosition = new Vector3(-0.02f, 0, 0);

        mountTransform = _torso.GetComponentInChildren<RightShoulder>().transform;           
        _rightArm = GameObject.Instantiate(_mechParts[3], mountTransform.position, Quaternion.Euler(-90, 0, 0), mountTransform);           
        _rightArmCont = _rightArm.GetComponent<ArmsController>();
        _rightArm.transform.localScale = new Vector3(-1, 1, 1);
        _rightArm.transform.localPosition = new Vector3(0.02f, 0, 0);

        /* Fill the list of all weapons on this unit (ORDER: rigth arm, left arm, rigth top,
            left top, rigth shoulder, left shoulder)*/
            
        mountTransform = _rightArm.GetComponentInChildren<HandMount>().transform;
        _mountedWeapons.Add(GameObject.Instantiate(_mechWeapons[0], mountTransform.position, mountTransform.rotation, mountTransform));
        WeaponList.Add(_mountedWeapons[0].GetComponentInChildren<WPNManager>());

        mountTransform = _leftArm.GetComponentInChildren<HandMount>().transform;
        _mountedWeapons.Add(GameObject.Instantiate(_mechWeapons[1], mountTransform.position, mountTransform.rotation, mountTransform));
        WeaponList.Add(_mountedWeapons[1].GetComponentInChildren<WPNManager>());

        mountTransform = _torso.GetComponentInChildren<RightMount>().transform;
        _mountedWeapons.Add(GameObject.Instantiate(_mechWeapons[2], mountTransform.position, mountTransform.rotation, mountTransform));
        WeaponList.Add(_mountedWeapons[2].GetComponentInChildren<WPNManager>());

        mountTransform = _torso.GetComponentInChildren<LeftMount>().transform;
        _mountedWeapons.Add(GameObject.Instantiate(_mechWeapons[3], mountTransform.position, mountTransform.rotation, mountTransform));
        WeaponList.Add(_mountedWeapons[3].GetComponentInChildren<WPNManager>());

        mountTransform = _rightArm.GetComponentInChildren<ShoulderMount>().transform;
        _mountedWeapons.Add(GameObject.Instantiate(_mechWeapons[4], mountTransform.position, mountTransform.rotation, mountTransform));
        WeaponList.Add(_mountedWeapons[4].GetComponentInChildren<WPNManager>());

        mountTransform = _leftArm.GetComponentInChildren<ShoulderMount>().transform;
        _mountedWeapons.Add(GameObject.Instantiate(_mechWeapons[5], mountTransform.position, mountTransform.rotation, mountTransform));
        WeaponList.Add(_mountedWeapons[5].GetComponentInChildren<WPNManager>());      

        // ??? assign unit manager for each weapon
        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {
                weapon.UnitManagerP = this;
                weapon.UnitID = transform.parent.name;
                WeaponCount += 1;

                if (transform.parent.name == "PlayerSquad")
                {
                    weapon.IsFriend = true;
                }
            }
        } 
    }

    // Start is called before the first frame update
    void Start()
    { 
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _clickMarker = GameObject.Find("ClickMarker"); 
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        _unitController = transform.GetComponentInParent<AIController>();
        ShrinkBar = GetComponentInChildren<ShrinkBar>();        
        UnitShield.ShieldID = transform.parent.name;
        UnitShield.UnitManagerP = this;       
        Cooling = CoolingModesP[0].Cooling;
        Heat = 1;    

        // Class parameters
        if (_unitClass == UnitClass.Scout)
        {
            WalkDistance = (int)(WalkDistance * 1.25f);
            SpreadMult = 0;
        }

        if (_unitClass == UnitClass.Heavy)
        {
            WalkDistance = (int)(WalkDistance * 0.75f);
            _armor = 0.75f;
        }       

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
            ShrinkBar.UpdateHeat();
        });
        
        ShrinkBar.UpdateStability();
        transform.LookAt(new Vector3(Target.transform.position.x, transform.position.y, Target.transform.position.z));
    }

    void Update()
    {
        MoveAnimation();

        // Arms animation ???
        _leftArmCont.Reset(0.1f); 
        _rightArmCont.Reset(0.1f); 

        if (_gameManager.InAction)
        {
            StartAction();            
        }

        ShrinkBar.UpdateShield();
        ShrinkBar.UpdateHealth();

        if (Spread > 0)
        {
            Spread -= Time.deltaTime;
            ShrinkBar.UpdateStability();
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
    private void StartAction()
    {       
        if (!IsDead)
        {            
            if (!Target.IsDead)
            {
                StartShoot();
            } 
            else
            {
                StopLaser();
            }
            
            // Heat dissipation
            if (Heat > 0)
            {
                Heat -= Cooling * Time.deltaTime;
                ShrinkBar.UpdateHeat();

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
        else
        {
            StopLaser();
        }
    }

    // Animate in Update ???
    private void MoveAnimation()
    {
        if (_gameManager.InAction && _unitController.UnitAgent.hasPath)
        {
            _legsController.Glide(IsLeft(transform.position, _unitController.UnitAgent.destination), 0.1f);
        }
        else if (!_unitController.UnitAgent.hasPath)
        {
            _legsController.Reset(0.1f);
        }
    }

    // If negative object to the , positive - to the left
    private bool IsLeft(Vector3 unitPos, Vector3 movePoint)
    {
        if (-unitPos.x * movePoint.z + unitPos.z * movePoint.x < 0)
        {
            return false;
        }
        else 
        {
            return true;
        }
    }


    private void StartShoot()
    {
        // Rotate body to Target
        _direction = Target.transform.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(_direction), 
            Time.time * _rotSpeed);

        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {
                weapon.FireBurst(Target);
            }
        }        
    }    

    private void StopLaser()
    {
        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {
                weapon.StopLaser();
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
            HP -= damage * _armor;            
        }

        // Death
        if (HP <= 0)
        {
            IsDead = true;            
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
            WalkDistance /= _coreParameters.MoveBoost; 
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
                _unitController.SetAgentDestination(_clickMarker.transform.position);
            }                
        }        
    }

    public void DisableShield()
    {
        UnitShield.DownTimer = 2;
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
                Random.Range((-MoveSpeed * SpreadMult) - Spread, (MoveSpeed * SpreadMult) + Spread),
                Random.Range((-MoveSpeed * SpreadMult) - Spread / 2, (MoveSpeed * SpreadMult) + Spread / 2),
                Random.Range((-MoveSpeed * SpreadMult) - Spread, (MoveSpeed * SpreadMult) + Spread));        
        return spread;
    }
    
    // Preview the heat for turn
    public void CalculateHeat()
    {        
        float heatCalculation = Heat; 
        foreach (WPNManager weapon in WeaponList)
        {
            if (weapon != null)
            {                    
                heatCalculation += weapon.CalculateHeat();
            }
        } 
        HeatCalc = heatCalculation - Cooling * _gameManager.TurnTime + UnitShield.Heat * _gameManager.TurnTime ;
        CoolOverdrive = false;        

        // Calculate auto cooling  
        if (Cooling == 0)
        {
            HeatCalc = heatCalculation + UnitShield.Heat * _gameManager.TurnTime 
                - CoolingModesP[0].Cooling * _gameManager.TurnTime;
            CoolOverdrive = false;

            if (HeatCalc >= HeatTreshold)
            {                    
                HeatCalc = heatCalculation + UnitShield.Heat * _gameManager.TurnTime 
                    - CoolingModesP[1].Cooling * _gameManager.TurnTime;
                CoolOverdrive = true;                        
            }
        }       
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
        CalculateHeat(); 
    }
}
