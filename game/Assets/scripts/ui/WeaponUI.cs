using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    //private GameManager gameManager;
    private UnitManager playerManager;

    [System.Serializable]
    private class WeaponButton
    {
        public TextMeshProUGUI weaponName;
        public SliderScr slider;
        public GameObject actionMask;

        public WeaponButton(TextMeshProUGUI newWeapon, SliderScr newSlider, GameObject newMask)
        {
            weaponName = newWeapon;
            slider = newSlider;
            actionMask = newMask;            
        }
    }

    private List<WeaponButton> weaponButtons;

    // Start is called before the first frame update
    void Start()
    {
        /* Fill the list of all weapon slots (ORDER: rigth arm, left arm, rigth top,
              left top, rigth shoulder, left shoulder) */
        weaponButtons = new List<WeaponButton>();
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightArmUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightArmUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftArmUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightTopUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightTopUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftTopUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").GetComponentInChildren<SliderScr>(), GameObject.Find("RightShoulderUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").GetComponentInChildren<SliderScr>(), GameObject.Find("LeftShoulderUI").transform.parent.Find("ActionMask").gameObject));

        UpdateUI();
    }

    // Update weapon UI when changing active unit
    public void UpdateUI()
    {
        playerManager = GameObject.Find("Player").GetComponentInChildren<UnitManager>();

        for (int i = 0; i < playerManager.weaponList.Count; i++ ) 
        {
            if (playerManager.weaponList[i] != null)
            {
                weaponButtons[i].weaponName.text = playerManager.weaponList[i].name;
                weaponButtons[i].slider.weapon = playerManager.weaponList[i];
                weaponButtons[i].slider.ChangeWPNmode();
                weaponButtons[i].actionMask.transform.localScale = Vector3.zero;
            }
            else 
            {
                weaponButtons[i].weaponName.transform.parent.localScale = Vector3.zero;
                weaponButtons[i].actionMask.transform.localScale = Vector3.one;
            }
        }
    }

    // Disable weapon
    public void WeaponDown(int wpnIndex, int downTimer)
    {
        weaponButtons[wpnIndex].actionMask.transform.localScale = Vector3.one;
        weaponButtons[wpnIndex].slider.slider.value = 0;        

        UpdateStatus(wpnIndex, downTimer);
    }

    // Enable weapon (update UI text)
    public void WeaponUp(int wpnIndex)
    {
        weaponButtons[wpnIndex].slider.modeName.text = playerManager.weaponList[wpnIndex]
            .weaponModes[(int)weaponButtons[wpnIndex].slider.slider.value].modeName;
    }

    // Update player weapon counters
    public void DecreaseCounter()
    {
        for (int i = 0; i < playerManager.weaponList.Count; i++)
        {
            if (playerManager.weaponList[i] != null && playerManager.weaponList[i].downTimer <= 0)
            {
                weaponButtons[i].actionMask.transform.localScale = Vector3.zero;
            }
        }
    }

    // Update weapon counter
    public void UpdateStatus(int wpnIndex, int downTimer)
    {
        if (downTimer == 1)
        {
            weaponButtons[wpnIndex].slider.modeName.text = "Off: " + downTimer + " trn";
        }
        else 
        {
            weaponButtons[wpnIndex].slider.modeName.text = "Off: " + downTimer + " trns";
        }     
    }
}
