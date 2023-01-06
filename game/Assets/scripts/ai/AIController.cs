using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{	
    private NavMeshAgent _unitAgent;
    private UnitManager _unitManager;
    private GameManager _gameManager;

    // Move parameters
    private readonly int _moveOffset = 15;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _unitAgent = GetComponent<NavMeshAgent>();
        _unitManager = GetComponentInChildren<UnitManager>();

		// ??? set target to shoot
		_unitManager.Target = GameObject.Find("PlayerSquad").transform.Find("Player").GetComponentInChildren<UnitManager>();
        _unitManager.UnitShield.ChangeMode(_unitManager.UnitShield.shieldModes[1]);            
    }   

    void Update()
    {
        // Cooling overdrive
        if (_unitManager.CoolingDownTimer <= 0 && _unitManager.Heat >= _unitManager.HeatTreshold 
        && _gameManager.InAction)
        {
            _unitManager.Cooling = _unitManager.CoolingModesP[1].Cooling; 
            _unitManager.CoolingOverdrive();
        }
    }

    // Action phase
    public void Move()
    {   
        SetPath();
        _unitAgent.speed = _unitManager.MoveSpeed * 1.1f;        
        _unitManager.UnitShield.TurnOnOff();   
        _unitManager.CoreOverdrive();             

        // Change fire modes depending on heat or enable weapon after overheat
        foreach (WPNManager weapon in _unitManager.WeaponList)
        {
            if (weapon != null && _unitManager.CoolingDownTimer > 3)
            {
                weapon.BurstSize = weapon.WeaponModesP[2].FireMode;
            }
            else if (weapon != null && weapon.DownTimer <= 0 && _unitManager.Heat < _unitManager.HeatTreshold)
            {
                int changeMode = Random.Range(1, weapon.WeaponModesP.Count);
                weapon.BurstSize = weapon.WeaponModesP[changeMode].FireMode;
            }
            else if (weapon != null && weapon.DownTimer <= 0 && _unitManager.Heat >= _unitManager.HeatTreshold)
            {
                int changeMode = Random.Range(0, weapon.WeaponModesP.Count - 1);
                weapon.BurstSize = weapon.WeaponModesP[changeMode].FireMode;
            }
        }        

        // Shield management
        if (_unitManager.UnitShield.DownTimer <= 0 && _unitManager.Heat < _unitManager.HeatTreshold)
        {         
            _unitManager.UnitShield.ChangeMode(_unitManager.UnitShield.shieldModes[2]);
            _unitManager.UnitShield.TurnOnOff();
        }
        else if (_unitManager.UnitShield.DownTimer <= 0)
        { 
            _unitManager.UnitShield.ChangeMode(_unitManager.UnitShield.shieldModes[1]);   
            _unitManager.UnitShield.TurnOnOff();
        }   
    }

    public void SetPath()
    {
        // Get distance between Target to avoid collision
        float targetDistance = Vector3.Distance(_unitAgent.destination, _unitManager.Target.transform.position);

        if (targetDistance > 10 && _unitManager.WalkDistance > 0) // ??? effective range
        {       
            _unitAgent.stoppingDistance = 0f;
            NavMeshPath path = new NavMeshPath();
            while (_unitManager.GetPathLength(path) < 0.5f && _unitManager.WalkDistance > 0.5f) // ??? movement behavior
            {
                _unitAgent.SetDestination(new Vector3(
                    _unitManager.Target.transform.position.x + Random.Range(-_moveOffset, _moveOffset),
                    _unitManager.Target.transform.position.y + Random.Range(-_moveOffset, _moveOffset),
                    _unitManager.Target.transform.position.z));
                NavMesh.CalculatePath(_unitAgent.transform.position, _unitAgent.destination, NavMesh.AllAreas, path);                
            }
            _unitManager.SetDestination(_unitAgent.destination, _unitAgent);     
            Debug.Log("long");       
        }
        else if (_unitManager.WalkDistance > 0)
        {        
            _unitAgent.stoppingDistance = 1f;      
            NavMeshPath path = new NavMeshPath();
            while (_unitManager.GetPathLength(path) < 0.5f && _unitManager.WalkDistance > 0.5f) // ??? movement behavior
            {
                _unitAgent.SetDestination(RandomNavmeshLocation(_unitManager.WalkDistance));
                NavMesh.CalculatePath(_unitAgent.transform.position, _unitAgent.destination, NavMesh.AllAreas, path);                
            }
            _unitManager.MoveSpeed = _unitManager.GetPathLength(path) / _gameManager.TurnTime;
            Debug.Log("short");  
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
        _unitManager.MoveSpeed = 0.1f;
        _unitManager.UpdateOverheatTimer();
    }
}
