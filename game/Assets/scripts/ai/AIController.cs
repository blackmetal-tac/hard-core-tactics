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

    public void Move()
    {
        _unitAgent.speed = _unitManager.moveSpeed + 0.5f;
        // Change fire modes depending on heat or enable weapon after overheat
        foreach (WPNManager weapon in _unitManager.weaponList)
        {
            if (weapon != null && weapon.downTimer <= 0 && _unitManager.heat < _unitManager.heatTreshold)
            {
                int changeMode = Random.Range(1, weapon.weaponModes.Count);
                weapon.burstSize = weapon.weaponModes[changeMode].fireMode;
            }
            else if (weapon != null && weapon.downTimer <= 0 && _unitManager.heat >= _unitManager.heatTreshold)
            {
                int changeMode = Random.Range(0, weapon.weaponModes.Count - 1);
                weapon.burstSize = weapon.weaponModes[changeMode].fireMode;
            }
        }        
    }
    public void EndMove()
    {
        _unitManager.moveSpeed = 0.1f;
        _unitManager.UpdateOverheatTimer();
    }
}
