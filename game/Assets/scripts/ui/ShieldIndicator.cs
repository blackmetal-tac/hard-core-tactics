using UnityEngine;
using UnityEngine.UI;

public class ShieldIndicator : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    private Image healthImage, damageImage;
    private GameManager gameManager;
    private UnitManager unitManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvasGroup = GetComponent<CanvasGroup>();

        unitManager = GameObject.Find("Player").transform.GetComponentInChildren<UnitManager>();
        healthImage = transform.Find("Health").GetComponent<Image>();
        damageImage = transform.Find("Damage").GetComponent<Image>();
    }

    public void UpdateShield()
    {
        healthImage.fillAmount = unitManager.shield;
        canvasGroup.alpha = gameManager.crosshairBars + ((1 - healthImage.fillAmount) / 2);

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
