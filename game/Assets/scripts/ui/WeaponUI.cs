using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    private GameManager gameManager;
    private UnitManager playerManager;

    [System.Serializable]
    private class WeaponButton
    {
        public TextMeshProUGUI weaponName, mode;
        public Slider slider;
        public GameObject actionMask;

        public WeaponButton(TextMeshProUGUI newWeapon, Slider newSlider, TextMeshProUGUI newMode, GameObject newMask)
        {
            weaponName = newWeapon;
            slider = newSlider;
            mode = newMode;
            actionMask = newMask;            
        }
    }

    private List<WeaponButton> weaponButtons;

    // Start is called before the first frame update
    void Start()
    {      
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();        

        /* Fill the list of all weapon slots (ORDER: rigth arm, left arm, rigth top,
              left top, rigth shoulder, left shoulder) */
        weaponButtons = new List<WeaponButton>();
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightArmUI").GetComponentInChildren<Slider>(), GameObject.Find("RightArmUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("RightArmUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").GetComponentInChildren<Slider>(), GameObject.Find("LeftArmUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightTopUI").GetComponentInChildren<Slider>(), GameObject.Find("RightTopUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("RightTopUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").GetComponentInChildren<Slider>(), GameObject.Find("LeftTopUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").GetComponentInChildren<Slider>(), GameObject.Find("RightShoulderUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").transform.parent.Find("ActionMask").gameObject));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").GetComponentInChildren<Slider>(), GameObject.Find("LeftShoulderUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").transform.parent.Find("ActionMask").gameObject));

        foreach (WeaponButton weaponButton in weaponButtons)
        {
            weaponButton.slider.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        }

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
                weaponButtons[i].actionMask.transform.localScale = Vector3.zero;
            }
            else 
            {
                weaponButtons[i].weaponName.transform.parent.localScale = Vector3.zero;
                weaponButtons[i].actionMask.transform.localScale = Vector3.one;
            }
        }   
        
        ChangeWPNmode();
    }

    private void ChangeWPNmode()
    {
        for (int i = 0; i < playerManager.weaponList.Count; i++)
        {
            if (playerManager.weaponList[i] != null)
            {
                playerManager.weaponList[i].burstSize = playerManager.weaponList[i]
                    .weaponModes[(int)weaponButtons[i].slider.value].fireMode;
            }
        }        

        if (gameManager.inAction)
        {
            foreach (WeaponButton button in weaponButtons)
            { 
                button.actionMask.transform.localScale = Vector3.one;
            } 
        }

        WeaponUp();
    }

    // Disable weapon
    public void WeaponDown(int wpnIndex, int downTimer)
    {
        weaponButtons[wpnIndex].actionMask.transform.localScale = Vector3.one;
        weaponButtons[wpnIndex].slider.value = 0;        

        UpdateStatus(wpnIndex, downTimer);
    }

    // Update weapon counter
    public void UpdateStatus(int wpnIndex, int downTimer)
    {
        weaponButtons[wpnIndex].mode.text = "Down: " + downTimer + " Turns";        
    }

    // Enable weapon (update UI text)
    public void WeaponUp()
    {
        for (int i = 0; i < playerManager.weaponList.Count; i++)
        {
            if (playerManager.weaponList[i] != null)
            {
                weaponButtons[i].mode.text = playerManager.weaponList[i]
                    .weaponModes[(int)weaponButtons[i].slider.value].modeName;
            }
        }
    }
}
