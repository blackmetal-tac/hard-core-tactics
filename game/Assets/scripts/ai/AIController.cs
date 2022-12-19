using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{	
    private NavMeshAgent _unitAgent;
    private UnitManager _unitManager;

    // Move parameters
    private readonly int _moveOffset = 15;

    // Start is called before the first frame update
    void Start()
    {
        _unitAgent = GetComponent<NavMeshAgent>();
        _unitManager = GetComponentInChildren<UnitManager>();
		// ??? set target to shoot
		_unitManager.Target = GameObject.Find("PlayerSquad").transform.Find("Player").gameObject;
        _unitManager.UnitShield.ChangeMode(_unitManager.UnitShield.shieldModes[1]);        
    }

    public void SetPath(NavMeshAgent Target)
    {
        _unitAgent.stoppingDistance = 0f;
        _unitAgent.SetDestination(new Vector3(
         Target.transform.position.x + Random.Range(-_moveOffset, _moveOffset),
         Target.transform.position.y + Random.Range(-_moveOffset, _moveOffset),
         Target.transform.position.z));

        // Get distance between Target to avoid collision
        float targetDistance = Vector3.Distance(_unitAgent.destination, Target.transform.position);
        float targetDestDist = Vector3.Distance(_unitAgent.destination, Target.destination);

        if (_unitAgent.destination == Target.transform.position || targetDistance < 1f 
            || targetDestDist < 1f)
        {
            _unitAgent.stoppingDistance = 1f;
        }
        _unitManager.SetDestination(_unitAgent.destination, _unitAgent);
    }

    // Action phase
    public void Move()
    {        
        _unitAgent.speed = _unitManager.MoveSpeed + 0.5f;
        _unitManager.UnitShield.TurnOnOff();        

        // Change fire modes depending on heat or enable weapon after overheat
        foreach (WPNManager weapon in _unitManager.WeaponList)
        {
            if (weapon != null && weapon.DownTimer <= 0 && _unitManager.Heat < _unitManager.HeatTreshold)
            {
                int changeMode = Random.Range(1, weapon.weaponModes.Count);
                weapon.BurstSize = weapon.weaponModes[changeMode].FireMode;
            }
            else if (weapon != null && weapon.DownTimer <= 0 && _unitManager.Heat >= _unitManager.HeatTreshold)
            {
                int changeMode = Random.Range(0, weapon.weaponModes.Count - 1);
                weapon.BurstSize = weapon.weaponModes[changeMode].FireMode;
            }
        }        

        // Shield management
        if (_unitManager.UnitShield.DownTimer <= 0 && _unitManager.Heat < _unitManager.HeatTreshold)
        {
            int changeMode = Random.Range(1, _unitManager.UnitShield.shieldModes.Count);
            _unitManager.UnitShield.ChangeMode(_unitManager.UnitShield.shieldModes[changeMode]);
        }
        else if (_unitManager.UnitShield.DownTimer <= 0 && _unitManager.Heat >= _unitManager.HeatTreshold)
        {
            int changeMode = Random.Range(0, _unitManager.UnitShield.shieldModes.Count - 1);
            _unitManager.UnitShield.ChangeMode(_unitManager.UnitShield.shieldModes[changeMode]);
        }        

        // Cooling overdrive
        if (_unitManager.CoolingDownTimer <= 0 && _unitManager.Heat >= _unitManager.HeatTreshold)
        {
            _unitManager.Cooling = _unitManager.coolingModes[1].Cooling;            
        }
        _unitManager.CoolingOverdrive();
    }
    
    public void EndMove()
    {
        _unitManager.MoveSpeed = 0.1f;
        _unitManager.UpdateOverheatTimer();
    }
}
