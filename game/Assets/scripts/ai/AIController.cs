using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using DG.Tweening;
using static GameManager;

public class AIController : MonoBehaviour
{	
    // Units formation
    public enum FormationType { Line, Arrow, Wedge, Staggered, Free }
    public FormationType UnitsFormation;
    [HideInInspector] public List<Transform> SquadPositions = new List<Transform>();
    private Transform _formationPos;    

    private PlayerInput _playerInput;
    private InputAction _leftClick;
    private WeaponUI _weaponUI;

    [HideInInspector] public NavMeshAgent UnitAgent;
    [HideInInspector] public UnitManager UnitManagerP;    
    
    private GameManager _gameManager;    
    private SquadManager _squadManager;
    private Camera _camMain; 
    private AIController _playerController, _enemyController;

    // Move parameters
    private GameObject _clickMarker, _crosshair;  
    [SerializeField] private LayerMask _ignoreLayers;
    private LineRenderer _walkPath;  
    private readonly int _moveOffset = 15;
    private bool _oneTime, _shieldEnable, _deathTrigger;

    private float _crosshairSize;
    private readonly float _crosshairScale = 0.15f;
    private static float _shieldTreshold = 0.25f;    

    void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _squadManager = transform.GetComponentInParent<SquadManager>();
        _playerInput = new PlayerInput();
        UnitManagerP = GetComponentInChildren<UnitManager>();
        UpdateManager();
        DefineFormation();
    }

    void OnEnable()
    {
        _leftClick = _playerInput.UI.LeftClick;
        _leftClick.Enable();
    }

    void OnDisable()
    {
        _leftClick.Disable();        
    }

    // Start is called before the first frame update
    void Start()
    {        
        if (transform.parent.name == "PlayerSquad")
        {
            _camMain = Camera.main;
            _crosshair = GameObject.Find("Crosshair");
            _walkPath = GetComponent<LineRenderer>();
            NavMesh.avoidancePredictionTime = 5;	

            // Path
            _clickMarker = GameObject.Find("ClickMarker");   
            _walkPath.startWidth = 0.02f;
            _walkPath.endWidth = 0.02f;
            _walkPath.positionCount = 0;	
        }

        UnitAgent = GetComponent<NavMeshAgent>();        
        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);     
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        SetUnitsPos();

        if(_formationPos != null)
        {            
            transform.position = _formationPos.position;
        }       
    }   

    void Update()
    {
        UnitDeathCheck();
        if (transform.parent.name == "PlayerSquad" && transform.name == _squadManager.AIControllers[_squadManager.CurrentUnit].name 
            && Time.time > _gameManager.LoadTime)
        {
            // Mouse click
            if (_leftClick.WasPressedThisFrame())
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {                     
                    return;
                }                                    

                if (!_gameManager.InAction)
                {
                    MoveToClick();
                }
            }

            // Draw player path
            DrawPath();

            // Swap dead unit
            UnitDeathCheck();

            if (!UnitManagerP.Target.IsDead)
            {
                // Dynamic _crosshair
                if (!_gameManager.InAction)
                {
                    _crosshairSize = Mathf.Lerp(_crosshairSize, _crosshairScale + UnitManagerP.MoveSpeed 
                        * UnitManagerP.SpreadMult * 1.5f, Time.deltaTime * 3);
                }
                else
                {
                    _crosshairSize = Mathf.Lerp(_crosshairSize, _crosshairScale + UnitManagerP.Spread / 8, Time.deltaTime * 3);                
                }
                _crosshair.transform.localScale = _crosshairSize * Vector3.one;

                // Crosshair position
                _crosshair.transform.position = _camMain.WorldToScreenPoint(UnitManagerP.Target.transform.position);
            }
            else
            {
                _crosshair.transform.localScale = Vector3.zero;
            }
        }
        else 
        {            
            if (transform.name == "Enemy")
            {
                // Cooling overdrive
                if (UnitManagerP.CoolingDownTimer <= 0 && UnitManagerP.Heat >= UnitManagerP.HeatTreshold 
                    && _gameManager.InAction)
                {
                    UnitManagerP.Cooling = UnitManagerP.CoolingModesP[1].Cooling; 
                    UnitManagerP.CoolingOverdrive();
                }                
            }

            // Keep shield from overload ---
            if (UnitManagerP.UnitShield.HP < _shieldTreshold && Time.time > _gameManager.LoadTime && !_oneTime
                && UnitManagerP.UnitShield.DownTimer <= 0)
            {      
                UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[0]);
                UnitManagerP.UnitShield.TurnOnOff();
                _oneTime = true;
            }

            if (UnitManagerP.UnitShield.HP > 0.5f && Time.time > _gameManager.LoadTime && !_shieldEnable
                && UnitManagerP.UnitShield.DownTimer <= 0 && UnitManagerP.UnitShield.Regeneration == 0
                && UnitManagerP.Heat < UnitManagerP.HeatTreshold)
            {           
                UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);   
                UnitManagerP.UnitShield.TurnOnOff();
                _shieldEnable = true;
            }
            else if (UnitManagerP.UnitShield.HP > 0.5f && Time.time > _gameManager.LoadTime && !_shieldEnable
                && UnitManagerP.UnitShield.DownTimer <= 0 && UnitManagerP.UnitShield.Regeneration == 0
                && UnitManagerP.Heat >= UnitManagerP.HeatTreshold)
            {            
                UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);   
                UnitManagerP.UnitShield.TurnOnOff();
                _shieldEnable = true;
            }

            if (UnitManagerP.UnitShield.HP > 0.5f && Time.time > _gameManager.LoadTime && !_shieldEnable
                && UnitManagerP.UnitShield.DownTimer <= 0
                && UnitManagerP.Heat >= UnitManagerP.HeatTreshold)
            {         
                UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);   
                UnitManagerP.UnitShield.TurnOnOff();
                _shieldEnable = true;
            }
            // ---
        }        
    }  

    // Swap dead unit
    private void UnitDeathCheck()
    {        
        if (UnitManagerP.IsDead && !_deathTrigger)
        {
            _squadManager.RemoveDeadUnit(this);
            _deathTrigger = true;
        }
    }

    private void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, 100, ~_ignoreLayers);               

        if (isHit)
        {   
            _clickMarker.transform.position = hit.point;
            _clickMarker.transform.localScale = Vector3.zero;
            _clickMarker.transform.DOScale(0.2f * Vector3.one , 0.5f);
            SetAgentDestination(hit.point);
            RotateAgent(hit.point);
        }       
    }

    private void RotateAgent(Vector3 point)
    {
        transform.LookAt(point);
        transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.y, 0));
        UnitManagerP.transform.LookAt(UnitManagerP.Target.transform);
    }

    // Set move position to maximum move distance (speed)
    public void SetAgentDestination(Vector3 movePoint)
    {        
        UnitAgent.speed = 0; 
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, movePoint, NavMesh.AllAreas, path);
        float pathLenght = GetPathLength(path);        
        for (int i = 0; i < path.corners.Length - 1; i++)
        {          
            if (pathLenght <= UnitManagerP.WalkDistance)
            {
                UnitAgent.SetDestination(movePoint);
                RotateAgent(movePoint);
                UnitManagerP.MoveSpeed = pathLenght / _gameManager.TurnTime;
                UnitManagerP.ShrinkBar.UpdateStability();                                      
            }
            else
            {
                Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * UnitManagerP.WalkDistance);
                UnitAgent.SetDestination(finalPoint);
                RotateAgent(finalPoint);
                UnitManagerP.MoveSpeed = UnitManagerP.WalkDistance / _gameManager.TurnTime;
                UnitManagerP.ShrinkBar.UpdateStability();
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

    // Draw player path
    public void DrawPath()
    {
        if (UnitAgent.hasPath)
        {
            _walkPath.startWidth = 0.02f;
            _walkPath.endWidth = 0.02f;
            _walkPath.positionCount = UnitAgent.path.corners.Length;
            _walkPath.SetPosition(0, transform.position);

            if (UnitAgent.path.corners.Length < 2)
            {
                return;
            } 

            for (int i = 1; i < UnitAgent.path.corners.Length; i++)
            {
                Vector3 pointPosition = new(UnitAgent.path.corners[i].x, UnitAgent.path.corners[i].y,
                        UnitAgent.path.corners[i].z);
                _walkPath.SetPosition(i, pointPosition);
                _clickMarker.transform.position = Vector3.MoveTowards(_clickMarker.transform.position, UnitAgent.destination, 
                Time.deltaTime * Vector3.Distance(_clickMarker.transform.position, UnitAgent.destination) * 4);
            }
            _clickMarker.transform.Rotate(new Vector3(0, 0, 50 * -Time.deltaTime));
        }      
    }

    public void ClearPath()
    {
        UnitAgent.ResetPath();

        if (_walkPath != null)
        {
            _walkPath.startWidth = 0;
            _walkPath.endWidth = 0;
            _clickMarker.transform.localScale = Vector3.zero;
        }        
    }

    // Action phase
    public void Move()
    {   
        if (UnitManagerP != null || !UnitManagerP.Target.IsDead)
        {

            UnitManagerP.UnitShield.TurnOnOff();
            UnitAgent.speed = UnitManagerP.MoveSpeed;
            _weaponUI.CoreButtonP.UpdateStatus();

            // AI behavior
            if (transform.name != "Player")    
            {
                // Active enemy unit
                if (transform.name == "Enemy")
                {
                    // Swap random units ???
                    if (_squadManager.SwitchCooldown <= 0)
                    {
                        _squadManager.CurrentUnit = Random.Range(0, _squadManager.AIControllers.Count);                    
                        _squadManager.ApplyPositions(false);
                    }
                    SetPath();

                    // Shield management
                    if (UnitManagerP.UnitShield.DownTimer <= 0 && UnitManagerP.Heat < UnitManagerP.HeatTreshold
                        && UnitManagerP.UnitShield.HP > _shieldTreshold)
                    {    
                        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[2]);
                        UnitManagerP.UnitShield.TurnOnOff();
                        _oneTime = false;
                        _shieldEnable = false;
                    }
                    else if (UnitManagerP.UnitShield.DownTimer <= 0 && UnitManagerP.UnitShield.HP > _shieldTreshold)
                    { 
                        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);   
                        UnitManagerP.UnitShield.TurnOnOff();
                        _oneTime = false;
                        _shieldEnable = false;
                    }                        
                    UnitManagerP.CoreOverdrive(); 

                    // Change fire modes depending on heat or enable weapon after overheat
                    foreach (WPNManager weapon in UnitManagerP.WeaponList)
                    {
                        if (weapon != null && weapon.DownTimer <= 0 && UnitManagerP.CoolingDownTimer > 3  
                            || weapon != null && weapon.DownTimer <= 0 && UnitManagerP.Heat < UnitManagerP.HeatTreshold / 2)
                        {
                            weapon.BurstSize = weapon.WeaponModesP[2].FireMode;
                        }
                        else if (weapon != null && weapon.DownTimer <= 0 && UnitManagerP.Heat < UnitManagerP.HeatTreshold)
                        {
                            int changeMode = Random.Range(1, weapon.WeaponModesP.Count);
                            weapon.BurstSize = weapon.WeaponModesP[changeMode].FireMode;
                        }
                        else if (weapon != null && weapon.DownTimer <= 0 && UnitManagerP.Heat >= UnitManagerP.HeatTreshold)
                        {
                            int changeMode = Random.Range(0, weapon.WeaponModesP.Count - 1);
                            weapon.BurstSize = weapon.WeaponModesP[changeMode].FireMode;
                        }
                    }            
                } 
                else
                {
                    if (UnitsFormation == FormationType.Free)
                    {
                        SetPath();
                    }

                    // Save your resources (passive units)
                    foreach (WPNManager weapon in UnitManagerP.WeaponList)
                    {
                        if (weapon != null && weapon.DownTimer <= 0 && weapon.ProjectileTypeP == WPNManager.ProjectileType.AMS)
                        {                       
                            weapon.BurstSize = weapon.WeaponModesP[weapon.WeaponModesP.Count - 1].FireMode;
                        }
                        else if (weapon != null && weapon.DownTimer <= 0)
                        {
                            int changeMode = Random.Range(0, weapon.WeaponModesP.Count - 1);
                            weapon.BurstSize = weapon.WeaponModesP[changeMode].FireMode;
                        }
                    }

                    // Shield management
                    if (UnitManagerP.UnitShield.DownTimer <= 0 && UnitManagerP.UnitShield.HP < 0.5f 
                        && UnitManagerP.Heat < UnitManagerP.HeatTreshold / 2)
                    {    
                        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[2]);
                        UnitManagerP.UnitShield.TurnOnOff();
                        _oneTime = false;
                        _shieldEnable = false;
                    }
                    else if (UnitManagerP.UnitShield.DownTimer <= 0)
                    { 
                        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);   
                        UnitManagerP.UnitShield.TurnOnOff();
                        _oneTime = false;
                        _shieldEnable = false;
                    }   
                }                 
            }
        } 
    }

    public void StartAction()
    {
        if (UnitsFormation != FormationType.Free && _playerController != null && _playerController.UnitAgent.speed > 0.1f)
        {
            KeepFormation();
        }

        if (UnitsFormation != FormationType.Free && _enemyController != null && _enemyController.UnitAgent.speed > 0.1f)
        {
            KeepFormation();
        }            
        
    }

    public void SetPath()
    {        
        // Get distance between Target to avoid collision
        float targetDistance = Vector3.Distance(UnitAgent.destination, UnitManagerP.Target.transform.position);        
        
        if (targetDistance > 10 && UnitManagerP.WalkDistance > 0) // ??? effective range
        {       
            UnitAgent.stoppingDistance = 0.1f;
            NavMeshPath path = new NavMeshPath();
            Vector3 destination = Vector3.zero;
            
            // Min movement range
            while (GetPathLength(path) < 0.5f && UnitManagerP.WalkDistance > 0.5f) // ??? movement behavior
            {           
                destination = new Vector3(
                    UnitManagerP.Target.transform.position.x + Random.Range(-_moveOffset, _moveOffset),
                    UnitManagerP.Target.transform.position.y + Random.Range(-_moveOffset, _moveOffset), 
                    UnitManagerP.Target.transform.position.z);
                NavMesh.CalculatePath(UnitAgent.transform.position, destination, NavMesh.AllAreas, path);                                   
            }  
            SetAgentDestination(destination);
            UnitAgent.speed = UnitManagerP.MoveSpeed;                                    
        }
        else if (UnitManagerP.WalkDistance > 0)
        {        
            UnitAgent.stoppingDistance = 1f;
            SetAgentDestination(RandomNavmeshLocation(UnitManagerP.WalkDistance)); 
            UnitAgent.speed = UnitManagerP.MoveSpeed;                       
        }
    }

    public void KeepFormation()
    {        
        if (_formationPos != null && UnitManagerP.WalkDistance > 0)
        {              
            UnitAgent.stoppingDistance = 0.1f;
            UnitAgent.SetDestination( _formationPos.position);
            SetSpeed();                                   
        } 
    }

    public void SetSpeed()
    {
        UnitManagerP.MoveSpeed = 1.3f * GetPathLength(UnitAgent.path) / _gameManager.TimeValue;            
        UnitAgent.speed = UnitManagerP.MoveSpeed;     
    }

    private Vector3 RandomNavmeshLocation(int radius) 
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1)) 
        {
            finalPosition = hit.position;            
        }
        return finalPosition;
    }

    public void SetUnitsPos()
    {
        if (_playerController != null)
        {  
            foreach (Transform position in _playerController.SquadPositions)
            {               
                if (transform.name == position.name)
                {                    
                    _formationPos = position;                    
                }
            }
        }

        if (_enemyController != null)
        {  
            foreach (Transform position in _enemyController.SquadPositions)
            {               
                if (transform.name == position.name)
                {                    
                    _formationPos = position;                    
                }               
            }
        }

        if (transform.name == "Player" || transform.name == "Enemy")
        {
            _formationPos = null;     
        }
    }

    public void SetTargets(string enemyTeam)
    {
        if (transform.name == "Bravo")
        {
            UnitManagerP.Target = GameObject.Find(enemyTeam).transform.Find("Charlie").GetComponentInChildren<UnitManager>();
        }
        if (transform.name == "Charlie")
        {
            UnitManagerP.Target = GameObject.Find(enemyTeam).transform.Find("Bravo").GetComponentInChildren<UnitManager>();
        }
        if (transform.name == "Delta")
        {
            UnitManagerP.Target = GameObject.Find(enemyTeam).transform.Find("Echo").GetComponentInChildren<UnitManager>();
        }
        if (transform.name == "Echo")
        {
            UnitManagerP.Target = GameObject.Find(enemyTeam).transform.Find("Delta").GetComponentInChildren<UnitManager>();
        }     

        // If not enough units or target is dead
        if (UnitManagerP.Target == null || UnitManagerP.Target.IsDead)
        {
            if (enemyTeam == "PlayerSquad")
            {
                UnitManagerP.Target = GameObject.Find(enemyTeam).transform.Find("Player").GetComponentInChildren<UnitManager>();
            }
            else
            {
                UnitManagerP.Target = GameObject.Find(enemyTeam).transform.Find("Enemy").GetComponentInChildren<UnitManager>();
            }
        }    
    }

    private void DefineFormation()
    {
        if (SquadPositions.Count == 0)
        {
            // Get squad positions
            SquadPositions.Add(transform.Find("Bravo").transform);
            SquadPositions.Add(transform.Find("Charlie").transform);
            SquadPositions.Add(transform.Find("Delta").transform);
            SquadPositions.Add(transform.Find("Echo").transform);
        }

        foreach (Formation formation in _gameManager.UnitsFormations)
        {
            if (formation.FormationName == UnitsFormation.ToString())
            {              
                for (int i = 0; i < formation.Positions.Length; i++)
                {
                    SquadPositions[i].SetParent(UnitManagerP.transform);
                    SquadPositions[i].localPosition = formation.Positions[i];
                }                
            }
        }
    }

    public void UpdateManager()
    { 
        // Set target to shoot
        if (transform.name == "Enemy")
        {
            UnitManagerP.Target = GameObject.Find("PlayerSquad").transform.Find("Player").GetComponentInChildren<UnitManager>();
        }
        
        if (transform.name == "Player")
        {
            UnitManagerP.Target = GameObject.Find("EnemySquad").transform.Find("Enemy").GetComponentInChildren<UnitManager>();
        }
        
        if (transform.parent.name == "EnemySquad")
        {
            _enemyController = transform.parent.Find("Enemy").GetComponent<AIController>(); 
            UnitsFormation = _enemyController.UnitsFormation;
            SetTargets("PlayerSquad");                    
        }		
        
        if (transform.parent.name == "PlayerSquad")
        {
            _playerController = transform.parent.Find("Player").GetComponent<AIController>();
            UnitsFormation = _playerController.UnitsFormation;
            SetTargets("EnemySquad");
        }   
    }
    
    public void EndMove()
    {
        UnitManagerP.MoveSpeed = 0.1f;               
        UnitManagerP.EndMove();

        if (transform.parent.name == "PlayerSquad")
        {
            ClearPath();
        }        
    }
}
