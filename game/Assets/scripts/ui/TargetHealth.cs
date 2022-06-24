using UnityEngine;
using UnityEngine.UI;

public class TargetHealth : MonoBehaviour
{
    public GameObject unit;
    private Image healthImage, damageImage;
    private UnitManager unitManager;

    // Start is called before the first frame update
    void Start()
    {
        unitManager = unit.GetComponent<UnitManager>();
        healthImage = transform.Find("Health").GetComponent<Image>();
        damageImage = transform.Find("Damage").GetComponent<Image>();
        damageImage.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        healthImage.fillAmount = unitManager.shield;

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
