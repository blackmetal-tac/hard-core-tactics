using UnityEngine;
using UnityEngine.UI;

public class ShrinkBar : MonoBehaviour
{
    private Camera _camMain;
    private Image _shieldImage, _shieldDmgImage, _shieldImageInd, _shieldDmgImageInd, _healthImage, _healthDmgImage,
        _healthImageInd, _healthDmgImageInd, _heatImage, _heatImageInd;
    private UnitManager _unitManager;
    private GameObject _unitUI, _shieldIndicator, _healthIndicator, _heatIndicator;
    private CanvasGroup _shieldCanvasGroup, _healthCanvasGroup, _heatCanvasGroup, _unitShieldGroup, _unitHealthGroup, 
        _unitHeatGroup;
    private GameManager _gameManager;
    private int _trasparencyMult = 5;
    private float _shrinkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _camMain = Camera.main;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _unitManager = transform.GetComponentInParent<UnitManager>();
        _unitUI = transform.Find("UnitUI").gameObject;
        _shrinkSpeed = 0.5f;

        // Health status Health
        // Shield
        _shieldImage = _unitUI.transform.Find("HP").Find("Shield").Find("Health").GetComponent<Image>();
        _shieldDmgImage = _shieldImage.transform.parent.Find("Damage").GetComponent<Image>();      
        _shieldIndicator = GameObject.Find("ShieldIndicator");
        _shieldCanvasGroup = _shieldIndicator.GetComponent<CanvasGroup>();
        _shieldImageInd = _shieldIndicator.transform.Find("Health").GetComponent<Image>();
        _shieldDmgImageInd = _shieldIndicator.transform.Find("Damage").GetComponent<Image>();
        _unitShieldGroup = _shieldImage.GetComponentInParent<CanvasGroup>();

        // Health
        _healthImage = _unitUI.transform.Find("HP").Find("Health").Find("Health").GetComponent<Image>();
        _healthDmgImage = _healthImage.transform.parent.Find("Damage").GetComponent<Image>();
        _healthIndicator = GameObject.Find("HealthIndicator");
        _healthCanvasGroup = _healthIndicator.GetComponent<CanvasGroup>();
        _healthImageInd = _healthIndicator.transform.Find("Health").GetComponent<Image>();
        _healthDmgImageInd = _healthIndicator.transform.Find("Damage").GetComponent<Image>();
        _unitHealthGroup = _healthImage.GetComponentInParent<CanvasGroup>();

        // Heat
        _heatImage = _unitUI.transform.Find("Heat").Find("Background").Find("Health").GetComponent<Image>();
        _heatIndicator = GameObject.Find("HeatIndicator");
        _heatCanvasGroup = _heatIndicator.GetComponent<CanvasGroup>();
        _heatImageInd = _heatIndicator.transform.Find("Health").GetComponent<Image>();
        _unitHeatGroup = _heatImage.GetComponentInParent<CanvasGroup>();
    }

    void Update()
    {
        _unitUI.transform.position = _camMain.WorldToScreenPoint(transform.parent.transform.position);
    }

    public void UpdateShield()
    {
        Shrink(_shieldImage, _shieldDmgImage, _unitManager.UnitShield.HP, true);
        _unitShieldGroup.alpha = _gameManager.CrosshairBars * _trasparencyMult + ((1 - _shieldImage.fillAmount) * 0.6f);

        // Player UI
        if (_unitManager.transform.parent.name == "Player")
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
        if (_unitManager.transform.parent.name == "Player")
        {
            _healthCanvasGroup.alpha = _gameManager.CrosshairBars + ((1 - _healthImage.fillAmount) * 0.6f);
            Shrink(_healthImageInd, _healthDmgImageInd, _unitManager.HP, false);
        }
    }

    public void UpdateHeat()
    {
        _heatImage.fillAmount = _unitManager.Heat;    
        _unitHeatGroup.alpha = _gameManager.CrosshairBars + (_unitManager.Heat * 0.9f);    

        if (_unitManager.transform.parent.name == "Player")
        {           
            _heatCanvasGroup.alpha = _gameManager.CrosshairBars + (_unitManager.Heat * 0.8f);
            _heatImageInd.fillAmount = _unitManager.Heat;
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
