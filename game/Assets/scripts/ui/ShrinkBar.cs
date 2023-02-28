using UnityEngine;
using UnityEngine.UI;

public class ShrinkBar : MonoBehaviour
{
    private Camera _camMain;
    private Image _shieldImage, _shieldDmgImage, _shieldImageInd, _shieldDmgImageInd, _healthImage, _healthDmgImage,
        _healthImageInd, _healthDmgImageInd, _heatImage, _heatImageInd, _stabImageInd, _heatCalculation, _heatThreshold;
    private UnitManager _unitManager;
    private GameObject _unitUI;
    private CanvasGroup _shieldCanvasGroup, _healthCanvasGroup, _heatCanvasGroup, _unitShieldGroup, _unitHealthGroup, 
        _unitHeatGroup, _stabCanvasGroup;
    private GameManager _gameManager;   
    private SquadManager _playerSquad; 
    private int _trasparencyMult = 5;
    private float _shrinkSpeed; 
    private bool _player;   

    void Awake()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _unitManager = transform.GetComponentInParent<UnitManager>();
        _unitUI = transform.Find("UnitUI").gameObject;        

        // Unit shield
        _shieldImage = _unitUI.transform.Find("HP").Find("Shield").Find("Health").GetComponent<Image>();
        _shieldDmgImage = _shieldImage.transform.parent.Find("Damage").GetComponent<Image>();
        _unitShieldGroup = _shieldImage.GetComponentInParent<CanvasGroup>();

        // Unit health
        _healthImage = _unitUI.transform.Find("HP").Find("Health").Find("Health").GetComponent<Image>();
        _healthDmgImage = _healthImage.transform.parent.Find("Damage").GetComponent<Image>();
        _unitHealthGroup = _healthImage.GetComponentInParent<CanvasGroup>();

        // Unit heat
        _heatImage = _unitUI.transform.Find("Heat").Find("Background").Find("Health").GetComponent<Image>();
        _heatCanvasGroup = GameObject.Find("HeatIndicator").GetComponent<CanvasGroup>();    
        _unitHeatGroup = _heatImage.GetComponentInParent<CanvasGroup>(); 

        if (_unitManager.transform.parent.name == "Player")
        {                        
            ToggleUI();
        }  
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _camMain = Camera.main;
        _shrinkSpeed = 0.5f;
        
        // Set references for UI
        if (_unitManager.transform.parent.parent.name == "PlayerSquad")
        {
            _playerSquad = _unitManager.transform.parent.GetComponentInParent<SquadManager>();

            // Player shield
            _shieldCanvasGroup = GameObject.Find("ShieldIndicator").GetComponent<CanvasGroup>();
            _shieldImageInd = GameObject.Find("ShieldIndicator").transform.Find("Health").GetComponent<Image>();
            _shieldDmgImageInd = GameObject.Find("ShieldIndicator").transform.Find("Damage").GetComponent<Image>();            
            
            // Player health
            _healthCanvasGroup = GameObject.Find("HealthIndicator").GetComponent<CanvasGroup>();
            _healthImageInd = GameObject.Find("HealthIndicator").transform.Find("Health").GetComponent<Image>();
            _healthDmgImageInd = GameObject.Find("HealthIndicator").transform.Find("Damage").GetComponent<Image>();            

            // Player heat
            _heatImageInd = GameObject.Find("HeatIndicator").transform.Find("Health").GetComponent<Image>();
            _heatCalculation = GameObject.Find("HeatIndicator").transform.Find("Calculation").GetComponent<Image>();
            _heatThreshold = GameObject.Find("HeatIndicator").transform.Find("Threshold").GetComponent<Image>();    
            _heatThreshold.fillAmount = 1 - _unitManager.HeatTreshold; 

            // Player stability 
            _stabCanvasGroup = GameObject.Find("Stability").GetComponent<CanvasGroup>();
            _stabImageInd = GameObject.Find("Stability").transform.Find("Health").GetComponent<Image>();  
        }
    }

    void Update()
    {
        _unitUI.transform.position = _camMain.WorldToScreenPoint(transform.parent.transform.position);
        if (_playerSquad != null && _playerSquad.AIControllers[_playerSquad.CurrentUnit].UnitManagerP == _unitManager
            && !_gameManager.InAction)
        {
            _heatCalculation.fillAmount = Mathf.Lerp(_heatCalculation.fillAmount,
                _unitManager.HeatCalc, Time.deltaTime * 2); 
            _heatCanvasGroup.alpha = _gameManager.CrosshairBars + (_heatCalculation.fillAmount * 0.8f);Debug.Log("HeatCalc - " + _unitManager.HeatCalc);                     
        }
        else if (_playerSquad != null && _gameManager.InAction)
        {
            _heatCalculation.fillAmount = 0;                
        }                         
    }

    public void UpdateShield()
    {
        Shrink(_shieldImage, _shieldDmgImage, _unitManager.UnitShield.HP, true);
        _unitShieldGroup.alpha = _gameManager.CrosshairBars * _trasparencyMult + ((1 - _shieldImage.fillAmount) * 0.6f);

        // Player UI
        if (_playerSquad != null && _playerSquad.AIControllers[_playerSquad.CurrentUnit].UnitManagerP == _unitManager)
        {
            _shieldCanvasGroup.alpha = _gameManager.CrosshairBars + ((1 - _shieldImage.fillAmount) * 0.6f);
            Shrink(_shieldImageInd, _shieldDmgImageInd, _unitManager.UnitShield.HP, true);
        }
    }

    public void UpdateHealth()
    {
        Shrink(_healthImage, _healthDmgImage, _unitManager.HP, false);
        _unitHealthGroup.alpha = _gameManager.CrosshairBars * _trasparencyMult + ((1 - _healthImage.fillAmount) * 0.6f);

        // Player UI
        if (_playerSquad != null && _playerSquad.AIControllers[_playerSquad.CurrentUnit].UnitManagerP == _unitManager)
        {
            _healthCanvasGroup.alpha = _gameManager.CrosshairBars + ((1 - _healthImage.fillAmount) * 0.6f);
            Shrink(_healthImageInd, _healthDmgImageInd, _unitManager.HP, false);
        }
    }

    public void UpdateHeat()
    {
        _heatImage.fillAmount = _unitManager.Heat;    
        _unitHeatGroup.alpha = _gameManager.CrosshairBars + (_unitManager.Heat * 0.9f);    

        if (_playerSquad != null && _playerSquad.AIControllers[_playerSquad.CurrentUnit].UnitManagerP == _unitManager)
        {
            _heatCanvasGroup.alpha = _gameManager.CrosshairBars + (_unitManager.Heat * 0.8f);
            _heatImageInd.fillAmount = _unitManager.Heat;Debug.Log("Heat - " + _unitManager.Heat);
        }
    }

    public void UpdateStability()
    {
        if (_playerSquad != null && _playerSquad.AIControllers[_playerSquad.CurrentUnit].UnitManagerP == _unitManager)
        { 
            float stab = _unitManager.Spread * 0.5f + _unitManager.MoveSpeed * (0.2f + 
                _playerSquad.AIControllers[_playerSquad.CurrentUnit].UnitManagerP.SpreadMult);
            _stabCanvasGroup.alpha = _gameManager.CrosshairBars + stab;
            _stabImageInd.fillAmount = 1 - stab;
        }
    }

    public void ToggleUI()
    {
        // Disable secondary bars for active unit        
        _player = !_player;
        if (_player)
        {
            _unitUI.transform.localScale = Vector3.zero;
        }
        else
        {
            _unitUI.transform.localScale = Vector3.one;
        }
    }

    private void Shrink(Image healthImage, Image damageImage, float healthValue, bool shield)
    {
        healthImage.fillAmount = healthValue;

        if (damageImage.fillAmount < healthImage.fillAmount)
        {
            damageImage.fillAmount = healthImage.fillAmount;
        }        

        if (shield)
        {
            // Health bar damage animation
            _unitManager.UnitShield.ShrinkTimer -= Time.deltaTime;
            if (_unitManager.UnitShield.ShrinkTimer < 0)
            {                
                if (healthImage.fillAmount < damageImage.fillAmount)
                {
                    damageImage.fillAmount -= _shrinkSpeed * Time.deltaTime;
                }
                else
                {
                    damageImage.fillAmount = healthImage.fillAmount;
                }
            }
        }
        else
        {
            // Health bar damage animation
            _unitManager.ShrinkTimer -= Time.deltaTime;
            if (_unitManager.ShrinkTimer < 0)
            {                
                if (healthImage.fillAmount < damageImage.fillAmount)
                {
                    damageImage.fillAmount -= _shrinkSpeed * Time.deltaTime;
                }
                else
                {
                    damageImage.fillAmount = healthImage.fillAmount;
                }
            }
        }
    }
}
