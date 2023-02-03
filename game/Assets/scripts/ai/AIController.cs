using UnityEngine;
using UnityEngine.AI;
using static GameManager;

public class AIController : MonoBehaviour
{	
    public enum FormationsList { Line, Arrow, Wedge, Staggered }
    public FormationsList UnitsFormation;
    [HideInInspector] public NavMeshAgent UnitAgent;
    [HideInInspector] public UnitManager UnitManagerP;
    private GameManager _gameManager;
    private Vector3 _formationPos;
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
        }
        else if (transform.parent.name == "EnemySquad")
        {
            _enemyController = transform.parent.Find("Enemy").GetComponent<AIController>(); 
            UnitManagerP.Target = GameObject.Find("PlayerSquad").transform.Find(transform.name).GetComponentInChildren<UnitManager>();
            SetUnitsPos();
        }		
        else if (transform.parent.name == "PlayerSquad")
        {
            _playerController = transform.parent.Find("Player").GetComponent<PlayerController>();
            UnitManagerP.Target = GameObject.Find("EnemySquad").transform.Find(transform.name).GetComponentInChildren<UnitManager>();
            SetUnitsPos();
        }
    }

    // Start is called before the first frame update
    void Start()
    {        
        UnitAgent = GetComponent<NavMeshAgent>();        
        UnitManagerP.UnitShield.ChangeMode(UnitManagerP.UnitShield.shieldModes[1]);            
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
            if (transform.name == "Enemy")
            {
                SetPath();
            }
            else
            {
                KeepFormation();
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
        if (_playerController != null && UnitManagerP.WalkDistance > 0)
        {       
            //Debug.Log(transform.name);
            UnitAgent.stoppingDistance = 0f;
            NavMeshPath path = new NavMeshPath();

            UnitAgent.SetDestination(new Vector3(
                _playerController.PlayerAgent.destination.x + _formationPos.x, //+ Random.Range(-_moveOffset / 10, _moveOffset / 10),
                _playerController.PlayerAgent.destination.y + _formationPos.y, //+ Random.Range(-_moveOffset / 10, _moveOffset / 10),
                _formationPos.z));
            NavMesh.CalculatePath(UnitAgent.transform.position, UnitAgent.destination, NavMesh.AllAreas, path);                

            UnitManagerP.SetDestination(UnitAgent.destination, UnitAgent);                        
        } 

        if (_enemyController != null && UnitManagerP.WalkDistance > 0)
        {       
            UnitAgent.stoppingDistance = 0f;
            NavMeshPath path = new NavMeshPath();

            UnitAgent.SetDestination(new Vector3(
                _enemyController.UnitAgent.destination.x + _formationPos.x, //+ Random.Range(-_moveOffset / 10, _moveOffset / 10),
                _enemyController.UnitAgent.destination.y + _formationPos.y, //+ Random.Range(-_moveOffset / 10, _moveOffset / 10),
                _formationPos.z));
            NavMesh.CalculatePath(UnitAgent.transform.position, UnitAgent.destination, NavMesh.AllAreas, path);                

            UnitManagerP.SetDestination(UnitAgent.destination, UnitAgent);                        
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
        if (transform.name == "Bravo")
        {
            DefineFormation(0);            
        }
        if (transform.name == "Charlie")
        {
            DefineFormation(1);
        }
    }

    private void DefineFormation(int index)
    {   
        foreach (Formation formation in _gameManager.UnitsFormations)
        {
            if (_playerController != null && formation.FormationName == _playerController.UnitsFormation.ToString())
            {
                _formationPos = formation.Positions[index];
                transform.position = _playerController.transform.position + _formationPos;
            }

            if (_enemyController != null && formation.FormationName == _enemyController.UnitsFormation.ToString())
            {
                _formationPos = formation.Positions[index];
                transform.position = _enemyController.transform.position + _formationPos;
            }
        }
    }
    
    public void EndMove()
    {
        UnitManagerP.MoveSpeed = 0.1f;        
        UnitManagerP.EndMove();
    }
}
