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
    private CanvasGroup _shieldUI, _coolingUI, _missileLockUI;
    private bool _bounce;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _actionMask = transform.parent.parent.Find("ActionMask").gameObject;
        SliderObject = GetComponent<Slider>();
        SliderObject.onValueChanged.AddListener(delegate { ChangeWPNmode(); });

        if (SliderObject.transform.parent.name == "ShieldUI")
        {
            _shieldUI = GameObject.Find("ShieldOverdriveUI").GetComponent<CanvasGroup>();
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {
            _coolingUI = GameObject.Find("CoolingOverdriveUI").GetComponent<CanvasGroup>();
            _missileLockUI = GameObject.Find("MissileLockUI").GetComponent<CanvasGroup>();
        }

        ModeName = transform.Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>();

        this.Wait(1, () =>{
            // Reset base cooling mode
            if (SliderObject.transform.parent.name == "CoolingUI")
            {
                SliderObject.value = 0;
            }     
        });   
    }

    void Update()
    {        
        if (SliderObject.transform.parent.name == "ShieldUI" && SliderObject.value == 2)
        {
            BounceUI(_shieldUI);            
        }
        else if (SliderObject.transform.parent.name == "ShieldUI")
        {
            _shieldUI.alpha = 0;
        }
        
        if (SliderObject.transform.parent.name == "CoolingUI" && SliderObject.value == 1 
            || SliderObject.transform.parent.name == "CoolingUI" && PlayerManager.Cooling == PlayerManager.CoolingModesP[1].Cooling)
        {
            BounceUI(_coolingUI);
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {
            _coolingUI.alpha = 0;
        }

        if (SliderObject.transform.parent.name == "CoolingUI")
        {
            if (PlayerManager.MissileLockTimer > 0)
            {
                BounceUI(_missileLockUI);
            }
            else
            {
                _missileLockUI.alpha = 0;
            }
        }

        // Auto Cooling overdrive
        if (SliderObject.transform.parent.name == "CoolingUI" && SliderObject.value == 2 && _gameManager.InAction)
        {
            if (PlayerManager.CoolingDownTimer <= 0 && PlayerManager.Heat >= PlayerManager.HeatTreshold)
            {
                PlayerManager.Cooling = PlayerManager.CoolingModesP[1].Cooling; 
                PlayerManager.CoolingOverdrive();                    
            }
        }
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
            PlayerManager.Cooling = PlayerManager.CoolingModesP[(int)SliderObject.value].Cooling;
            ModeName.text = PlayerManager.CoolingModesP[(int)SliderObject.value].ModeName;            

            if (_gameManager.InAction)
            {                
                if (PlayerManager.Cooling == PlayerManager.CoolingModesP[1].Cooling)
                {
                    PlayerManager.CoolingOverdrive();
                }
                else
                {
                    _actionMask.transform.localScale = Vector3.one;
                }                
            }

            if (SliderObject.value == 2)
            {
                PlayerManager.AutoCooling = true;
            }
            else
            {
                PlayerManager.AutoCooling = false;
            }
        }
        else
        {
            Weapon.BurstSize = Weapon.WeaponModesP[(int)SliderObject.value].FireMode;
            ModeName.text = Weapon.WeaponModesP[(int)SliderObject.value].ModeName;
            Weapon.ChangeShotsCount(); // shots for burst laser

            if (_gameManager.InAction)
            {
                Weapon.LastBurst = 0f;
                _actionMask.transform.localScale = Vector3.one;
            }
        }
    }

    private void BounceUI(CanvasGroup canvasGroup)
    {        
        if (!_bounce)
        {
            canvasGroup.alpha += Time.deltaTime;
        }
        else if (_bounce)
        {
            canvasGroup.alpha -= Time.deltaTime;
        }

        if (canvasGroup.alpha <= 0.1f)
        {
            _bounce = false;
        }
        else if (canvasGroup.alpha >= 1)
        {
            _bounce = true;            
        }
    }
}
