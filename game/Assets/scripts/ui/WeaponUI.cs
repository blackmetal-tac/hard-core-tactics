using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    private UnitManager _playerManager;
    public CoreButton CoreButtonP;

    [System.Serializable]
    private class WeaponButton
    {
        public TextMeshProUGUI WeaponName;
        public SliderScr Slider;
        public ActionMask ActionMask;

        public WeaponButton(TextMeshProUGUI newWeapon, SliderScr newSlider, ActionMask newMask)
        {
            WeaponName = newWeapon;
            Slider = newSlider;
            ActionMask = newMask;            
        }
    }

    private List<WeaponButton> _weaponButtons = new List<WeaponButton>();    

    // Start is called before the first frame update
    void Start()
    {
        /* Fill the list of all weapon slots (ORDER: rigth arm, left arm, rigth top,
              left top, rigth shoulder, left shoulder) */        
        _weaponButtons.Add(new WeaponButton(GameObject.Find("RightArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightArmUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightArmUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("LeftArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftArmUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("RightTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightTopUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightTopUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("LeftTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftTopUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("RightShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightShoulderUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("LeftShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftShoulderUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));

        // Add other modules (6+ id's)
        _weaponButtons.Add(new WeaponButton(GameObject.Find("ShieldUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("ShieldUI").GetComponentInChildren<SliderScr>(), GameObject.Find("ShieldUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("CoolingUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("CoolingUI").GetComponentInChildren<SliderScr>(), GameObject.Find("CoolingUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        
        CoreButtonP = GameObject.Find("CoreButton").GetComponent<CoreButton>();
        UpdateUI();
    }

    // Update weapon UI when changing active unit
    public void UpdateUI()
    {
        _playerManager = GameObject.Find("Player").GetComponentInChildren<UnitManager>();
        CoreButtonP.PlayerManager = _playerManager;        

        // Shield controls
        _weaponButtons[6].WeaponName.text = _playerManager.UnitShield.name;
        _weaponButtons[6].Slider.PlayerManager = _playerManager;
        _weaponButtons[6].Slider.ChangeWPNmode();

        // Cooling controls
        _weaponButtons[7].WeaponName.text = "Cooling System";
        _weaponButtons[7].Slider.PlayerManager = _playerManager;
        _weaponButtons[7].Slider.ChangeWPNmode();

        // Weapons controls
        for (int i = 0; i < _playerManager.WeaponList.Count; i++ ) 
        {
            if (_playerManager.WeaponList[i] != null)
            {
                _weaponButtons[i].WeaponName.text = _playerManager.WeaponList[i].name;
                _weaponButtons[i].Slider.Weapon = _playerManager.WeaponList[i];
                _weaponButtons[i].Slider.ChangeWPNmode();             
            }
            else 
            {
                _weaponButtons[i].WeaponName.transform.parent.localScale = Vector3.zero;
                _weaponButtons[i].ActionMask.transform.localScale = Vector3.one;
            }
        }
    }

    // Disable weapon
    public void WeaponDown(int wpnIndex, int downTimer)
    {
        if (wpnIndex == 7 && !_playerManager.AutoCooling)
        {
            _weaponButtons[wpnIndex].Slider.SliderObject.value = 1;
        }
        else if (wpnIndex != 7)
        {
            _weaponButtons[wpnIndex].Slider.SliderObject.value = 0; 
        }
        _weaponButtons[wpnIndex].ActionMask.transform.localScale = Vector3.one;              

        UpdateStatus(wpnIndex, downTimer);
    }

    // Enable weapon (update UI text)
    public void WeaponUp(int wpnIndex)
    {        
        _weaponButtons[wpnIndex].Slider.ChangeWPNmode();     
    }

    // Update player weapon counters
    public void DecreaseCounter()
    {
        for (int i = 0; i < _playerManager.WeaponList.Count; i++)
        {
            if (_playerManager.WeaponList[i] != null && _playerManager.WeaponList[i].DownTimer <= 0)
            {
                _weaponButtons[i].ActionMask.transform.localScale = Vector3.zero;
            }
        }

        if (_playerManager.UnitShield.DownTimer <= 0) 
        {
            _weaponButtons[6].ActionMask.transform.localScale = Vector3.zero;
        }
        
        if (_playerManager.CoolingDownTimer <= 0)
        {
            _weaponButtons[7].ActionMask.transform.localScale = Vector3.zero;
        }      
        else if (_playerManager.CoolingDownTimer > 3 && _playerManager.AutoCooling)
        {
            _weaponButtons[7].ActionMask.transform.localScale = Vector3.one;
        }  
        else if (_playerManager.CoolingDownTimer == 3 && !_playerManager.AutoCooling)
        {
            _weaponButtons[7].Slider.SliderObject.value = 0;
            UpdateStatus(7, 3);
        }
        else if (_playerManager.CoolingDownTimer == 3 && _playerManager.AutoCooling)
        {            
            UpdateStatus(7, 3);
        }
    }

    // Update weapon counter
    public void UpdateStatus(int wpnIndex, int downTimer)
    {
        if (wpnIndex == 7 && downTimer > 3)
        {
            _weaponButtons[wpnIndex].Slider.ModeName.text = "Overdrive";
        }
        else
        {
            if (downTimer == 1)
            {
                _weaponButtons[wpnIndex].Slider.ModeName.text = "Off: " + downTimer + " turn";
            }
            else 
            {
                _weaponButtons[wpnIndex].Slider.ModeName.text = "Off: " + downTimer + " trns";
            }     
        }
    }
}
