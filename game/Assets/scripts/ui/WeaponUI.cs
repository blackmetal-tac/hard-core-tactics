using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    private WPNManager rightWPN;
    private GameManager gameManager;

    private class WeaponButton
    {
        public TextMeshProUGUI weaponName, mode;
        public Slider slider;
        public ActionMask actionMask;

        public WeaponButton(TextMeshProUGUI newWeapon, Slider newSlider, TextMeshProUGUI newMode, ActionMask newMask)
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
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightArmUI").GetComponentInChildren<Slider>(), GameObject.Find("RightArmUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("RightArmUI").transform.parent.GetComponentInChildren<ActionMask>()));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftArmUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").GetComponentInChildren<Slider>(), GameObject.Find("LeftArmUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("LeftArmUI").transform.parent.GetComponentInChildren<ActionMask>()));
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightTopUI").GetComponentInChildren<Slider>(), GameObject.Find("RightTopUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("RightTopUI").transform.parent.GetComponentInChildren<ActionMask>()));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftTopUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").GetComponentInChildren<Slider>(), GameObject.Find("LeftTopUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("LeftTopUI").transform.parent.GetComponentInChildren<ActionMask>()));
        weaponButtons.Add(new WeaponButton(GameObject.Find("RightShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").GetComponentInChildren<Slider>(), GameObject.Find("RightShoulderUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("RightShoulderUI").transform.parent.GetComponentInChildren<ActionMask>()));
        weaponButtons.Add(new WeaponButton(GameObject.Find("LeftShoulderUI").transform.Find("Weapon").GetComponent<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").GetComponentInChildren<Slider>(), GameObject.Find("LeftShoulderUI").transform.Find("Slider").Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>(), GameObject.Find("LeftShoulderUI").transform.parent.GetComponentInChildren<ActionMask>()));

        foreach (WeaponButton weaponButton in weaponButtons)
        {
            weaponButton.slider.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (WPNManager weapon in we)

        rightWPN = GameObject.Find("Player").transform.Find("Body").Find("Torso").Find("RightArm")
            .Find("RightArmWPN").GetComponentInChildren<WPNManager>();        

        weaponName.text = rightWPN.name;
        
        ChangeWPNmode();
    }

    private void ChangeWPNmode()
    {
        rightWPN.burstSize = rightWPN.weaponModes[(int)rightWPNui.value].fireMode;
        WeaponUp();

        if (gameManager.inAction)
        {
            weaponMasks[0].transform.localScale = Vector3.one;
        }
    }

    public void WeaponDown(int wpnIndex, int downTimer)
    {
        weaponMasks[wpnIndex].transform.localScale = Vector3.one;
        rightWPNui.value = 0;
        UpdateStatus(downTimer);
    }

    public void UpdateStatus(int downTimer)
    {
        modeText.text = "Down: " + downTimer + " Turns";
    }

    public void WeaponUp()
    {
        modeText.text = rightWPN.weaponModes[(int)rightWPNui.value].modeName;
    }
}
