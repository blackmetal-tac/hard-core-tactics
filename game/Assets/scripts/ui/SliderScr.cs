using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScr : MonoBehaviour
{
    private GameObject actionMask;
    private GameManager gameManager;
    public Slider slider { get; set; }
    public WPNManager weapon { get; set; }
    public TextMeshProUGUI modeName { get; set; }

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
        weapon.burstSize = weapon.weaponModes[(int)slider.value].fireMode;
        modeName.text = weapon.weaponModes[(int)slider.value].modeName;

        if (gameManager.inAction)
        {
            weapon.lastBurst = 0f;
            actionMask.transform.localScale = Vector3.one;
        }
    }
}
