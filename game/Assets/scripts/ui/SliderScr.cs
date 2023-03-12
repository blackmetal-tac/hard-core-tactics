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
    private ParametersUI _shieldParam, _coolingParam, _overheatParam, _heatCalcParam, _missileLockUIParam, _amsUIParam; 

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
            _shieldParam = GameObject.Find("ShieldOverdriveUI").GetComponent<ParametersUI>();
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {
            _coolingUI = GameObject.Find("CoolingOverdriveUI").GetComponent<CanvasGroup>();
            _coolingParam = GameObject.Find("CoolingOverdriveUI").GetComponent<ParametersUI>();
            _overheatUI = GameObject.Find("OverheatUI").GetComponent<CanvasGroup>();
            _overheatParam = GameObject.Find("OverheatUI").GetComponent<ParametersUI>();
            _heatCalc = GameObject.Find("HeatIndicator").transform.Find("Calculation").GetComponent<CanvasGroup>();
            _heatCalcParam = GameObject.Find("HeatIndicator").transform.Find("Calculation").GetComponent<ParametersUI>();
            _missileLockUI = GameObject.Find("MissileLockUI").GetComponent<CanvasGroup>();       
            _missileLockUIParam = GameObject.Find("MissileLockUI").GetComponent<ParametersUI>();      
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
                _amsUIParam = GameObject.Find("AMSUI").GetComponent<ParametersUI>();               
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
                BounceUI(_amsUI, _amsUIParam);
            }
            else
            {
                _amsUI.alpha = 0;
            }
        }

        if (_shieldUI != null && SliderObject.value == 2)
        {
            BounceUI(_shieldUI, _shieldParam);            
        }
        else if (_shieldUI != null)
        {
            _shieldUI.alpha = 0;
        }        
        
        if (_coolingUI != null)
        {
            if (PlayerManager.Cooling == PlayerManager.CoolingModesP[1].Cooling 
                || PlayerManager.CoolOverdrive)
            {
                BounceUI(_coolingUI, _coolingParam);
            }
            else if (PlayerManager.Cooling != PlayerManager.CoolingModesP[1].Cooling 
                && !PlayerManager.CoolOverdrive)
            {
                _coolingUI.alpha = 0;
            }
        }

        if (_coolingUI != null)
        {
            if (PlayerManager.MissileLockTimer > 0)
            {
                BounceUI(_missileLockUI, _missileLockUIParam);
            }
            else
            {
                _missileLockUI.alpha = 0;
            }
        }

        if (_coolingUI != null )
        {
            if (PlayerManager.Heat > PlayerManager.HeatTreshold 
                || PlayerManager.HeatCalc >= PlayerManager.HeatTreshold)
            {
                BounceUI(_overheatUI, _overheatParam);
            }            
            else
            {
                _overheatUI.alpha = 0;
            }
        }

        if (_coolingUI != null && !_gameManager.InAction)
        {
            BounceUI(_heatCalc, _heatCalcParam);
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

    public void UpdateUI()
    {
        if (_shieldUI != null)
        {
            for (int i= 0; i < PlayerManager.UnitShield.shieldModes.Count; i++)
            {
                if (PlayerManager.UnitShield.shieldModes[i].Regen == PlayerManager.UnitShield.Regeneration)
                {
                    SliderObject.value = i;
                    ChangeMask(PlayerManager.UnitShield.DownTimer);
                }                 
            }            
        }
        else if (_coolingUI != null)
        {
            for (int i= 0; i < PlayerManager.CoolingModesP.Count; i++)
            {
                if (PlayerManager.CoolingModesP[i].Cooling == PlayerManager.Cooling)
                {
                    SliderObject.value = i;
                    ChangeMask(PlayerManager.CoolingDownTimer);
                }                 
            }        
        }  
        else
        {
            for (int i= 0; i < Weapon.WeaponModesP.Count; i++)
            {
                if (Weapon.WeaponModesP[i].FireMode == Weapon.BurstSize)
                {
                    SliderObject.value = i;
                    ChangeMask(Weapon.DownTimer);
                }                 
            }  
        }     
    }

    private void ChangeMask(int timer)
    {
        if (Time.time > _gameManager.LoadTime && timer <= 0)
        {
            _actionMask.transform.localScale = Vector3.zero;
        }
        else
        {
            _actionMask.transform.localScale = Vector3.one;
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

    private void BounceUI(CanvasGroup canvasGroup, ParametersUI param)
    {        
        if (!param.Bounce)
        {
            canvasGroup.alpha += Time.deltaTime;            
        }
        else 
        {
            canvasGroup.alpha -= Time.deltaTime;            
        }

        if (canvasGroup.alpha <= 0.1f)
        {
            param.Bounce = false;
        }
        else if (canvasGroup.alpha >= 1)
        {
            param.Bounce = true;            
        }
    }
}
