using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScr : MonoBehaviour
{
    private GameObject _actionMask;
    private GameManager _gameManager;
    [HideInInspector] public Slider SliderObject;
    [HideInInspector] public WPNManager Weapon;
    [HideInInspector] public TextMeshProUGUI ModeName;
    [HideInInspector] public Shield UnitShield;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _actionMask = transform.parent.parent.Find("ActionMask").gameObject;
        SliderObject = GetComponent<Slider>();
        SliderObject.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        ModeName = transform.Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>();
    }

    public void ChangeWPNmode()
    {
        if (SliderObject.transform.parent.name == "ShieldUI")
        {
            UnitShield.Regeneration = UnitShield.shieldModes[(int)SliderObject.value].Regen;
            UnitShield.Heat = UnitShield.shieldModes[(int)SliderObject.value].Heat;
            ModeName.text = UnitShield.shieldModes[(int)SliderObject.value].ModeName;
            UnitShield.TurnOnOff();

            if (_gameManager.InAction)
            {                
                _actionMask.transform.localScale = Vector3.one;
            }
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {

        }
        else
        {
            Weapon.BurstSize = Weapon.weaponModes[(int)SliderObject.value].FireMode;
            ModeName.text = Weapon.weaponModes[(int)SliderObject.value].ModeName;
            Weapon.ChangeShotsCount(); // shots for burst laser

            if (_gameManager.InAction)
            {
                Weapon.LastBurst = 0f;
                _actionMask.transform.localScale = Vector3.one;
            }
        }
    }
}
