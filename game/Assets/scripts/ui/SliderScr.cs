using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScr : MonoBehaviour
{
    private GameObject actionMask;
    private GameManager gameManager;
    [HideInInspector] public Slider slider;
    [HideInInspector] public WPNManager weapon;
    [HideInInspector] public TextMeshProUGUI modeName;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        actionMask = transform.parent.parent.Find("ActionMask").gameObject;
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        modeName = transform.Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ChangeWPNmode()
    {
        weapon.BurstSize = weapon.weaponModes[(int)slider.value].fireMode;
        modeName.text = weapon.weaponModes[(int)slider.value].modeName;
        weapon.ChangeShotsCount();

        if (gameManager.inAction)
        {
            weapon.lastBurst = 0f;
            actionMask.transform.localScale = Vector3.one;
        }
    }
}
