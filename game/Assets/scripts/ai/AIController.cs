using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using static GameManager;

public class AIController : MonoBehaviour
{	
    public enum FormationType { Line, Arrow, Wedge, Staggered, Free }
    public FormationType UnitsFormation;
    [HideInInspector] public NavMeshAgent UnitAgent;
    [HideInInspector] public UnitManager UnitManagerP;
    [HideInInspector] public List<Transform> SquadPositions = new List<Transform>();
    private Transform _formationPos;
    private GameManager _gameManager;    
    private PlayerController _playerController;
    private AIController _enemyController;

    // Move parameters
    private readonly int _moveOffset = 15;
    private bool _oneTime, _shieldEnable;
    private static float _shieldTreshold = 0.25f;

    void Awake()
    {
        UnitManagerP = GetComponentInChildren<UnitManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Set target to shoot
        if (transform.name == "Enemy")
        {
            UnitManagerP.Target = GameObject.Find("PlayerSquad").transform.Find("Player").GetComponentInChildren<UnitManager>();
            DefineFormation();
        }
        else if (transform.parent.name == "EnemySquad")
        {
            _enemyController = transform.parent.Find("Enemy").GetComponent<AIController>(); 
            UnitsFormation = _enemyController.UnitsFormation;
            SetTargets("PlayerSquad");        
        }		
        else if (transform.parent.name == "PlayerSquad")
        {
            _playerController = transform.parent.Find("Player").GetComponent<PlayerController>();
            UnitsFormation = (FormationType)_playerController.UnitsFormation;
            SetTargets("EnemySquad");
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        UnitAgent = GetComponent<NavMeshAgent>();        
        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);     

        if(transform.name != "Enemy")
        {
            SetUnitsPos();
        }       
    }   

    void Update()
    {
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

        // Cooling overdrive
        if (UnitManagerP.CoolingDownTimer <= 0 && UnitManagerP.Heat >= UnitManagerP.HeatTreshold 
        && _gameManager.InAction)
        {
            UnitManagerP.Cooling = UnitManagerP.CoolingModesP[1].Cooling; 
            UnitManagerP.CoolingOverdrive();
        }
    }

    // Action phase
    public void Move()
    {   
        if (UnitManagerP != null || !UnitManagerP.Target.IsDead)
        {
            if (transform.name == "Enemy" || UnitsFormation == FormationType.Free)
            {
                SetPath();
            }
            UnitAgent.speed = UnitManagerP.MoveSpeed;        
            UnitManagerP.UnitShield.TurnOnOff();   
            UnitManagerP.CoreOverdrive();             

            // Change fire modes depending on heat or enable weapon after overheat
            foreach (WPNManager weapon in UnitManagerP.WeaponList)
            {
                if (weapon != null && UnitManagerP.CoolingDownTimer > 3)
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
        } 
    }

    public void StartAction()
    {
        if (transform.name != "Enemy" || UnitsFormation != FormationType.Free)
        {
            KeepFormation();
        }
        UnitManagerP.StartAction();
    }

    public void SetPath()
    {
        // Get distance between Target to avoid collision
        float targetDistance = Vector3.Distance(UnitAgent.destination, UnitManagerP.Target.transform.position);

        if (targetDistance > 10 && UnitManagerP.WalkDistance > 0) // ??? effective range
        {       
            UnitAgent.stoppingDistance = 0f;
            NavMeshPath path = new NavMeshPath();
            while (UnitManagerP.GetPathLength(path) < 0.5f && UnitManagerP.WalkDistance > 0.5f) // ??? movement behavior
            {
                UnitAgent.SetDestination(new Vector3(
                    UnitManagerP.Target.transform.position.x + Random.Range(-_moveOffset, _moveOffset),
                    UnitManagerP.Target.transform.position.y + Random.Range(-_moveOffset, _moveOffset),
                    UnitManagerP.Target.transform.position.z));
                NavMesh.CalculatePath(UnitAgent.transform.position, UnitAgent.destination, NavMesh.AllAreas, path);                
            }
            UnitManagerP.SetDestination(UnitAgent.destination, UnitAgent);                        
        }
        else if (UnitManagerP.WalkDistance > 0)
        {        
            UnitAgent.stoppingDistance = 1f;      
            NavMeshPath path = new NavMeshPath();
            while (UnitManagerP.GetPathLength(path) < 0.5f && UnitManagerP.WalkDistance > 0.5f) // ??? movement behavior
            {
                UnitAgent.SetDestination(RandomNavmeshLocation(UnitManagerP.WalkDistance));
                NavMesh.CalculatePath(UnitAgent.transform.position, UnitAgent.destination, NavMesh.AllAreas, path);                
            }
            UnitManagerP.MoveSpeed = UnitManagerP.GetPathLength(path) / _gameManager.TurnTime;             
        }
    }

    public void KeepFormation()
    {        
        if (transform.name != "Enemy" && UnitManagerP.WalkDistance > 0)
        {  
            UnitAgent.stoppingDistance = 0f;            
            UnitAgent.SetDestination(new Vector3(
                _formationPos.position.x + Random.Range(-1, 1),
                _formationPos.position.y + Random.Range(-1, 1),
                _formationPos.position.z));

            UnitManagerP.MoveSpeed = 2 * UnitManagerP.GetPathLength(UnitAgent.path) / _gameManager.TimeValue;            
            UnitAgent.speed = UnitManagerP.MoveSpeed;                                   
        } 
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

    private void SetUnitsPos()
    {
        if (_playerController != null)
        {  
            foreach (Transform position in _playerController.SquadPositions)
            {               
                if (transform.name == position.name)
                {                    
                    _formationPos = position;
                    transform.position = _formationPos.position;
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
                    transform.position = _formationPos.position;
                }
            }
        }
    }

    private void SetTargets(string enemyTeam)
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
    }

    private void DefineFormation()
    {
        // Get squad positions
        SquadPositions.Add(transform.Find("Bravo").transform);
        SquadPositions.Add(transform.Find("Charlie").transform);
        SquadPositions.Add(transform.Find("Delta").transform);
        SquadPositions.Add(transform.Find("Echo").transform);

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
    
    public void EndMove()
    {
        UnitManagerP.MoveSpeed = 0.1f;        
        UnitManagerP.EndMove();
    }
}
