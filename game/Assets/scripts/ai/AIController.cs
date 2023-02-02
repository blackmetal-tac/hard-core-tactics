using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{	
    private NavMeshAgent _unitAgent;
    [HideInInspector] public UnitManager UnitManagerP;
    private GameManager _gameManager;

    // Move parameters
    private readonly int _moveOffset = 15;
    private bool _oneTime, _shieldEnable;
    private static float _shieldTreshold = 0.25f;

    void Awake()
    {
        UnitManagerP = GetComponentInChildren<UnitManager>();

        // Set target to shoot
        if (transform.name == "Enemy")
        {
            UnitManagerP.Target = GameObject.Find("PlayerSquad").transform.Find("Player").GetComponentInChildren<UnitManager>();
        }
        else if (transform.parent.name == "EnemySquad")
        {
            UnitManagerP.Target = GameObject.Find("PlayerSquad").transform.Find(transform.name).GetComponentInChildren<UnitManager>();
        }		
        else if (transform.parent.name == "PlayerSquad")
        {
            UnitManagerP.Target = GameObject.Find("EnemySquad").transform.Find(transform.name).GetComponentInChildren<UnitManager>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _unitAgent = GetComponent<NavMeshAgent>();        
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
            SetPath();
            _unitAgent.speed = UnitManagerP.MoveSpeed;        
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
        float targetDistance = Vector3.Distance(_unitAgent.destination, UnitManagerP.Target.transform.position);

        if (targetDistance > 10 && UnitManagerP.WalkDistance > 0) // ??? effective range
        {       
            _unitAgent.stoppingDistance = 0f;
            NavMeshPath path = new NavMeshPath();
            while (UnitManagerP.GetPathLength(path) < 0.5f && UnitManagerP.WalkDistance > 0.5f) // ??? movement behavior
            {
                _unitAgent.SetDestination(new Vector3(
                    UnitManagerP.Target.transform.position.x + Random.Range(-_moveOffset, _moveOffset),
                    UnitManagerP.Target.transform.position.y + Random.Range(-_moveOffset, _moveOffset),
                    UnitManagerP.Target.transform.position.z));
                NavMesh.CalculatePath(_unitAgent.transform.position, _unitAgent.destination, NavMesh.AllAreas, path);                
            }
            UnitManagerP.SetDestination(_unitAgent.destination, _unitAgent);                        
        }
        else if (UnitManagerP.WalkDistance > 0)
        {        
            _unitAgent.stoppingDistance = 1f;      
            NavMeshPath path = new NavMeshPath();
            while (UnitManagerP.GetPathLength(path) < 0.5f && UnitManagerP.WalkDistance > 0.5f) // ??? movement behavior
            {
                _unitAgent.SetDestination(RandomNavmeshLocation(UnitManagerP.WalkDistance));
                NavMesh.CalculatePath(_unitAgent.transform.position, _unitAgent.destination, NavMesh.AllAreas, path);                
            }
            UnitManagerP.MoveSpeed = UnitManagerP.GetPathLength(path) / _gameManager.TurnTime;             
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
    
    public void EndMove()
    {
        UnitManagerP.MoveSpeed = 0.1f;        
        UnitManagerP.EndMove();
    }
}
