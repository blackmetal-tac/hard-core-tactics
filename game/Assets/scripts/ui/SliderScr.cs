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
    private CanvasGroup _shieldUI, _coolingUI, _overheatUI, _heatCalc, _missileLockUI, _amsUI;
    private bool _bounce;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _actionMask = transform.parent.parent.Find("ActionMask").gameObject;
        SliderObject = GetComponent<Slider>();
        SliderObject.onValueChanged.AddListener(delegate { ChangeWPNmode(); });        

        // Sliders UI indicators
        if (SliderObject.transform.parent.name == "ShieldUI")
        {
            _shieldUI = GameObject.Find("ShieldOverdriveUI").GetComponent<CanvasGroup>();
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {
            _coolingUI = GameObject.Find("CoolingOverdriveUI").GetComponent<CanvasGroup>();
            _overheatUI = GameObject.Find("OverheatUI").GetComponent<CanvasGroup>();
            _heatCalc = GameObject.Find("HeatIndicator").transform.Find("Calculation").GetComponent<CanvasGroup>();
            _missileLockUI = GameObject.Find("MissileLockUI").GetComponent<CanvasGroup>();          
        }
        
        ModeName = transform.Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>();
        
        this.Wait(1, () =>{
            // Reset base cooling mode
            if (_coolingUI != null)
            {
                SliderObject.value = 0;
            }  

            if (Weapon != null && Weapon.ProjectileTypeP == WPNManager.ProjectileType.AMS)
            {
                _amsUI = GameObject.Find("AMSUI").GetComponent<CanvasGroup>();                
            }   
        });   

        this.Wait(_gameManager.LoadTime, () => {
            if (_shieldUI != null || _coolingUI != null)
            {
                PlayerManager.CalculateHeat();
            }
            else
            {
                Weapon.UnitManagerP.CalculateHeat();
            }
        });
    }

    void Update()
    {       
        if (_amsUI != null)
        {            
            if (SliderObject.value > 0)
            {
                BounceUI(_amsUI);
            }
            else
            {
                _amsUI.alpha = 0;
            }
        }

        if (_shieldUI != null && SliderObject.value == 2)
        {
            BounceUI(_shieldUI);            
        }
        else if (_shieldUI != null)
        {
            _shieldUI.alpha = 0;
        }        
        
        if (_coolingUI != null && PlayerManager.Cooling == PlayerManager.CoolingModesP[1].Cooling)
        {
            BounceUI(_coolingUI);
        }
        else if (_coolingUI != null)
        {
            _coolingUI.alpha = 0;
        }

        if (_coolingUI != null)
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

        if (_coolingUI != null )
        {
            if (PlayerManager.Heat > PlayerManager.HeatTreshold 
                || PlayerManager.HeatCalc > PlayerManager.HeatTreshold)
            {
                BounceUI(_overheatUI);
            }            
            else
            {
                _overheatUI.alpha = 0;
            }
        }

        if (_coolingUI != null && !_gameManager.InAction)
        {
            BounceUI(_heatCalc);
        }        

        // Auto Cooling overdrive
        if (_coolingUI != null && SliderObject.value == 2 && _gameManager.InAction)
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
        if (_shieldUI != null)
        {
            PlayerManager.UnitShield.ChangeMode(PlayerManager.UnitShield.shieldModes[(int)SliderObject.value]);
            ModeName.text = PlayerManager.UnitShield.shieldModes[(int)SliderObject.value].ModeName;
            PlayerManager.UnitShield.TurnOnOff();

            if (Time.time > _gameManager.LoadTime)
            {
                PlayerManager.CalculateHeat();
            }            

            if (_gameManager.InAction)
            {                
                _actionMask.transform.localScale = Vector3.one;
            }
        }
        else if (_coolingUI != null)
        {            
            PlayerManager.Cooling = PlayerManager.CoolingModesP[(int)SliderObject.value].Cooling;
            ModeName.text = PlayerManager.CoolingModesP[(int)SliderObject.value].ModeName; 

            if (Time.time > _gameManager.LoadTime)
            {
                PlayerManager.CalculateHeat();
            }          

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

            if (Time.time > _gameManager.LoadTime)
            {
                Weapon.UnitManagerP.CalculateHeat();   
            }       

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
