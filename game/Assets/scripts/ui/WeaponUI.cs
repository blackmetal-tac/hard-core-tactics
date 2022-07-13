using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    private WPNManager rightWPN;

    // Start is called before the first frame update
    void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        rightWPN = GameObject.Find("Player").transform.Find("Body").Find("Torso").Find("RightArm")
            .Find("RightArmWPN").GetComponentInChildren<WPNManager>(); 
    }
}
