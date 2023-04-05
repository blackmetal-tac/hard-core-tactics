using UnityEngine;
using System.Collections.Generic;

public class SquadManager : MonoBehaviour
{
    [HideInInspector] public List<AIController> AIControllers = new List<AIController>(); 
    [HideInInspector] public int CurrentUnit;
    private GameManager _gameManager;
    private CameraMovement _cameraMov;
    private WeaponUI _weaponUI;
    private int _prevUnit; 
    [HideInInspector] public int SwitchCooldown; 
    
    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {            
            AIControllers.Add(transform.GetChild(i).GetComponent<AIController>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _cameraMov = Camera.main.GetComponent<CameraMovement>();
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
    } 

    public void SwitchUnit(bool deadUnit)
    {
        if (AIControllers.Count > 1)
        {
            AIControllers[CurrentUnit].ClearPath();
            if (CurrentUnit < AIControllers.Count - 1)
            {
                CurrentUnit++;
                AIControllers[CurrentUnit - 1].UnitManagerP.ShrinkBar.ToggleUI();
                AIControllers[CurrentUnit].UnitManagerP.ShrinkBar.ToggleUI();            
            }
            else
            {
                CurrentUnit = 0;
                AIControllers[AIControllers.Count - 1].UnitManagerP.ShrinkBar.ToggleUI();
                AIControllers[CurrentUnit].UnitManagerP.ShrinkBar.ToggleUI();            
            }        

            // Move camera to next unit
            if (!deadUnit)
            {
                _cameraMov.Destination = AIControllers[CurrentUnit].gameObject;
            }            

            // Update UI for next player unit
            if (AIControllers[CurrentUnit].transform.parent.name == "PlayerSquad")
            {                
                _weaponUI.UpdateUI(AIControllers[CurrentUnit].name);
            }
        }
    }

    public void ApplyPositions(bool deadUnit)
    {
        if (CurrentUnit != _prevUnit && SwitchCooldown <= 0 || deadUnit)
        {  
            string prevUnit = AIControllers[_prevUnit].name;

            // Swap dead unit
            AIControllers[CurrentUnit].UnitAgent.path = AIControllers[_prevUnit].UnitAgent.path;
            AIControllers[CurrentUnit].SetSpeed();

            AIControllers[_prevUnit].name = AIControllers[CurrentUnit].name;
            AIControllers[CurrentUnit].name = prevUnit;

            foreach (AIController controller in AIControllers)
            {
                controller.UpdateManager();
                controller.SetUnitsPos();
            }

            if (transform.name == "PlayerSquad")
            {                
                foreach (AIController controller in _gameManager.EnemySquad.AIControllers)
                {
                    controller.UpdateManager();                    
                }
            }
            else
            {
                foreach (AIController controller in _gameManager.PlayerSquad.AIControllers)
                {
                    controller.UpdateManager();                    
                }
            }
            
            _prevUnit = CurrentUnit;
            if (!deadUnit)
            {                
                SwitchCooldown = 2;
            }
        }
    }

    // Swap dead unit
    public void RemoveDeadUnit(AIController _deadUnit)
    {
        if (_deadUnit.name == "Player" && AIControllers.Count > 1 || _deadUnit.name == "Enemy" && AIControllers.Count > 1)
        {                            
            SwitchUnit(true);
            ApplyPositions(true); 
            AIControllers.Remove(_deadUnit);            
            _deadUnit.UnitAgent.enabled = false;

            if (CurrentUnit >= AIControllers.Count)
            {
                CurrentUnit = AIControllers.Count - 1;
                _prevUnit = CurrentUnit;
            } 
        }     
        else if (AIControllers.Count > 1)
        {
            AIControllers.Remove(_deadUnit);  
            _deadUnit.UnitAgent.enabled = false;         
        }
    }  

    public void KillTarget()
    {
        if(_gameManager.InAction)
        {
            AIControllers[CurrentUnit].UnitManagerP.Target.TakeDamage(1);
        }
    }
}
