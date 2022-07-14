using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    private WPNManager rightWPN;
    private Slider rightWPNui;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        rightWPN = GameObject.Find("Player").transform.Find("Body").Find("Torso").Find("RightArm")
            .Find("RightArmWPN").GetComponentInChildren<WPNManager>();
        rightWPNui = transform.Find("RightButtons").Find("RightArm").Find("UI")
            .GetComponentInChildren<Slider>();

        rightWPNui.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        ChangeWPNmode();
    }

    private void ChangeWPNmode()
    {
        rightWPN.burstSize = rightWPN.weaponModes[(int)rightWPNui.value].fireMode;
    }
}
