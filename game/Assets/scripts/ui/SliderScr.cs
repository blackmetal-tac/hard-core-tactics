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
    [HideInInspector] public UnitManager PlayerManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _actionMask = transform.parent.parent.Find("ActionMask").gameObject;
        SliderObject = GetComponent<Slider>();
        SliderObject.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        ModeName = transform.Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>();

        this.Wait(1, () =>{
            // Reset base cooling mode
            if (SliderObject.transform.parent.name == "CoolingUI")
            {
                SliderObject.value = 0;
            }     
        });   
    }

    public void ChangeWPNmode()
    {
        if (SliderObject.transform.parent.name == "ShieldUI")
        {
            PlayerManager.UnitShield.ChangeMode(PlayerManager.UnitShield.shieldModes[(int)SliderObject.value]);
            ModeName.text = PlayerManager.UnitShield.shieldModes[(int)SliderObject.value].ModeName;
            PlayerManager.UnitShield.TurnOnOff();

            if (_gameManager.InAction)
            {                
                _actionMask.transform.localScale = Vector3.one;
            }
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {            
            PlayerManager.Cooling = PlayerManager.coolingModes[(int)SliderObject.value].Cooling;
            ModeName.text = PlayerManager.coolingModes[(int)SliderObject.value].ModeName;

            if (_gameManager.InAction)
            {                
                if (PlayerManager.Cooling == PlayerManager.coolingModes[1].Cooling)
                {
                    PlayerManager.CoolingOverdrive();
                }
                else
                {
                    _actionMask.transform.localScale = Vector3.one;
                }                
            }
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
