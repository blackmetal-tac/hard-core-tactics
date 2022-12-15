using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    private UnitManager _playerManager;

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

    private List<WeaponButton> _weaponButtons;    

    // Start is called before the first frame update
    void Start()
    {
        /* Fill the list of all weapon slots (ORDER: rigth arm, left arm, rigth top,
              left top, rigth shoulder, left shoulder) */
        _weaponButtons = new List<WeaponButton>();
        _weaponButtons.Add(new WeaponButton(GameObject.Find("RightArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightArmUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightArmUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("LeftArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftArmUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("RightTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightTopUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightTopUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("LeftTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftTopUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("RightShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightShoulderUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("LeftShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftShoulderUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));

        // Add other modules (6+ id's)
        _weaponButtons.Add(new WeaponButton(GameObject.Find("ShieldUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("ShieldUI").GetComponentInChildren<SliderScr>(), GameObject.Find("ShieldUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));
        _weaponButtons.Add(new WeaponButton(GameObject.Find("CoolingUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("CoolingUI").GetComponentInChildren<SliderScr>(), GameObject.Find("CoolingUI").transform.parent.Find("ActionMask").GetComponent<ActionMask>()));

        UpdateUI();
    }

    // Update weapon UI when changing active unit
    public void UpdateUI()
    {
        _playerManager = GameObject.Find("Player").GetComponentInChildren<UnitManager>();

        // Shield controls
        _weaponButtons[6].WeaponName.text = _playerManager.UnitShield.name;
        _weaponButtons[6].Slider.UnitShield = _playerManager.UnitShield;
        _weaponButtons[6].Slider.ChangeWPNmode();

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
        _weaponButtons[wpnIndex].ActionMask.transform.localScale = Vector3.one;
        _weaponButtons[wpnIndex].Slider.SliderObject.value = 0;        

        UpdateStatus(wpnIndex, downTimer);
    }

    // Enable weapon (update UI text)
    public void WeaponUp(int wpnIndex)
    {
        if (wpnIndex < 6)
        {
            _weaponButtons[wpnIndex].Slider.ModeName.text = _playerManager.WeaponList[wpnIndex]
                .weaponModes[(int)_weaponButtons[wpnIndex].Slider.SliderObject.value].ModeName;
        }
        else if (wpnIndex == 6)
        {
            _weaponButtons[wpnIndex].Slider.ModeName.text = 
                _playerManager.UnitShield.shieldModes[(int)_weaponButtons[wpnIndex].Slider.SliderObject.value].ModeName;
        }
        
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
    }

    // Update weapon counter
    public void UpdateStatus(int wpnIndex, int downTimer)
    {
        if (downTimer == 1)
        {
            _weaponButtons[wpnIndex].Slider.ModeName.text = "Off: " + downTimer + " trn";
        }
        else 
        {
            _weaponButtons[wpnIndex].Slider.ModeName.text = "Off: " + downTimer + " trns";
        }     
    }
}
