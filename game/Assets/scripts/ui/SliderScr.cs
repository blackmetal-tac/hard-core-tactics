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
    private CanvasGroup _shieldUI, _coolingUI;
    private bool _shieldOver, _coolingOver;

    private Material _material;
    public Color color;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _actionMask = transform.parent.parent.Find("ActionMask").gameObject;
        _shieldUI = GameObject.Find("ShieldOverdriveUI").GetComponent<CanvasGroup>();
        _coolingUI = GameObject.Find("CoolingOverdriveUI").GetComponent<CanvasGroup>();
        SliderObject = GetComponent<Slider>();
        SliderObject.onValueChanged.AddListener(delegate { ChangeWPNmode(); });
        ModeName = transform.Find("Handle Slide Area").Find("Handle").GetComponentInChildren<TextMeshProUGUI>();
        _material = _coolingUI.GetComponentInChildren<MeshRenderer>().material;

        this.Wait(1, () =>{
            // Reset base cooling mode
            if (SliderObject.transform.parent.name == "CoolingUI")
            {
                SliderObject.value = 0;
            }     
        });   
    }

    void FixedUpdate()
    {
        //_material.color = color;
        if (SliderObject.transform.parent.name == "ShieldUI" && SliderObject.value == 2)
        {
            BounceUI(_shieldUI);            
        }
        else if (SliderObject.transform.parent.name == "ShieldUI")
        {
            _shieldUI.alpha = 0;
        }
        
        if (SliderObject.transform.parent.name == "CoolingUI" && SliderObject.value == 1 )
            //|| PlayerManager.Cooling == PlayerManager.coolingModes[1].Cooling)
        {
            BounceUI(_coolingUI);
        }
        else if (SliderObject.transform.parent.name == "CoolingUI")
        {
            _coolingUI.alpha = 0;
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

    private void BounceUI(CanvasGroup canvasGroup)
    {        
        if (!_shieldOver)
        {
            canvasGroup.alpha += Time.deltaTime;
        }
        else if (_shieldOver)
        {
            canvasGroup.alpha -= Time.deltaTime;
        }

        if (canvasGroup.alpha <= 0.1f)
        {
            _shieldOver = false;
        }
        else if (canvasGroup.alpha >= 1)
        {
            _shieldOver = true;            
        }
    }
}
