using UnityEngine;
using UnityEngine.UI;

public class ShrinkBar : MonoBehaviour
{
    private Camera camMain;
    private Image shieldImage, shieldDmgImage, shieldImageInd, shieldDmgImageInd, healthImage, healthDmgImage,
        healthImageInd, healthDmgImageInd, heatImage, heatImageInd;
    private UnitManager unitManager;
    private GameObject unitUI, shieldIndicator, healthIndicator, heatIndicator;
    private CanvasGroup shieldCanvasGroup, healthCanvasGroup, heatCanvasGroup;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        unitManager = transform.GetComponentInParent<UnitManager>();
        unitUI = transform.Find("UnitUI").gameObject;

        // Health status Health
        // Shield
        shieldImage = unitUI.transform.Find("Background").Find("Shield").Find("Health").GetComponent<Image>();
        shieldDmgImage = shieldImage.transform.parent.Find("Damage").GetComponent<Image>();      
        shieldIndicator = GameObject.Find("ShieldIndicator");
        shieldCanvasGroup = shieldIndicator.GetComponent<CanvasGroup>();
        shieldImageInd = shieldIndicator.transform.Find("Health").GetComponent<Image>();
        shieldDmgImageInd = shieldIndicator.transform.Find("Damage").GetComponent<Image>();

        // Health
        healthImage = unitUI.transform.Find("Background").Find("Health").Find("Health").GetComponent<Image>();
        healthDmgImage = healthImage.transform.parent.Find("Damage").GetComponent<Image>();
        healthIndicator = GameObject.Find("HealthIndicator");
        healthCanvasGroup = healthIndicator.GetComponent<CanvasGroup>();
        healthImageInd = healthIndicator.transform.Find("Health").GetComponent<Image>();
        healthDmgImageInd = healthIndicator.transform.Find("Damage").GetComponent<Image>();

        // Heat
        heatImage = unitUI.transform.Find("Heat").Find("Background").Find("Health").GetComponent<Image>();
        heatIndicator = GameObject.Find("HeatIndicator");
        heatCanvasGroup = heatIndicator.GetComponent<CanvasGroup>();
        heatImageInd = heatIndicator.transform.Find("Health").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        unitUI.transform.position = camMain.WorldToScreenPoint(transform.parent.transform.position);
    }

    public void UpdateShield()
    {
        Shrink(shieldImage, shieldDmgImage, unitManager.shield);

        if (unitManager.transform.parent.name == "Player")
        {
            shieldCanvasGroup.alpha = gameManager.crosshairBars + ((1 - shieldImage.fillAmount) / 2);
            Shrink(shieldImageInd, shieldDmgImageInd, unitManager.shield);
        }
    }

    public void UpdateHealth()
    {
        Shrink(healthImage, healthDmgImage, unitManager.HP);

        if (unitManager.transform.parent.name == "Player")
        {
            healthCanvasGroup.alpha = gameManager.crosshairBars + ((1 - healthImage.fillAmount) / 2);
            Shrink(healthImageInd, healthDmgImageInd, unitManager.HP);
        }
    }

    public void UpdateHeat()
    {
        heatImage.fillAmount = unitManager.heat;        

        if (unitManager.transform.parent.name == "Player")
        {           
            heatCanvasGroup.alpha = gameManager.crosshairBars + (unitManager.heat * 0.8f);
            heatImageInd.fillAmount = unitManager.heat;
        }
    }

    private void Shrink(Image healthImage, Image damageImage, float healthValue)
    {
        healthImage.fillAmount = healthValue;

        // Health bar damage animation
        unitManager.shrinkTimer -= Time.deltaTime;
        if (unitManager.shrinkTimer < 0)
        {
            float shrinkSpeed = 1f;
            if (healthImage.fillAmount < damageImage.fillAmount)
            {
                damageImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
            else
            {
                damageImage.fillAmount = healthImage.fillAmount;
            }
        }
    }
}
