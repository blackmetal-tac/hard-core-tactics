using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUI : MonoBehaviour
{
    private WPNManager rightWPN;
    private Slider rightWPNui;
    private TextMeshProUGUI weaponName;
    private TextMeshProUGUI modeText;

    // Start is called before the first frame update
    void Start()
    {
        rightWPNui = GameObject.Find("RightArmUI").GetComponentInChildren<Slider>();
        weaponName = rightWPNui.transform.parent.Find("Weapon").GetComponent<TextMeshProUGUI>();
        modeText = GameObject.Find("RightArmMode").GetComponent<TextMeshProUGUI>();
        rightWPNui.onValueChanged.AddListener(delegate { ChangeWPNmode(); });

        UpdateUI();
    }

    public void UpdateUI()
    {
        rightWPN = GameObject.Find("Player").transform.Find("Body").Find("Torso").Find("RightArm")
            .Find("RightArmWPN").GetComponentInChildren<WPNManager>();

        weaponName.text = rightWPN.name;
        
        ChangeWPNmode();
    }

    private void ChangeWPNmode()
    {
        rightWPN.burstSize = rightWPN.weaponModes[(int)rightWPNui.value].fireMode;
        modeText.text = rightWPN.weaponModes[(int)rightWPNui.value].modeName;
    }
}
