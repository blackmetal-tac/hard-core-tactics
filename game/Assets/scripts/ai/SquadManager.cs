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
    
    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _cameraMov = Camera.main.GetComponent<CameraMovement>();
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        for (int i = 0; i < transform.childCount; i++)
        {            
            AIControllers.Add(transform.GetChild(i).GetComponent<AIController>());
        }
    } 

    public void SwitchUnit()
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
        _cameraMov.Destination = AIControllers[CurrentUnit].gameObject;

        // Update UI for new unit
        _weaponUI.UpdateUI(AIControllers[CurrentUnit].name);
    }

    public void ApplyPositions()
    {
        if (CurrentUnit != _prevUnit && SwitchCooldown <= 0)
        {  
            string prevUnit = AIControllers[_prevUnit].name;
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
            _gameManager.UpdateTargets = true;
            SwitchCooldown = 2;
        }
    }
}
