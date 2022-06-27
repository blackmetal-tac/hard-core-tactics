using UnityEngine;
using UnityEngine.UI;

public class ShrinkBar : MonoBehaviour
{
    private Camera camMain;
    private Image healthImage, damageImage;
    private UnitManager unitManager;
    private GameObject unitUI;

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        unitManager = transform.GetComponentInParent<UnitManager>();
        unitUI = transform.GetChild(0).gameObject;
        healthImage = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetComponent<Image>();
        damageImage = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        unitUI.transform.position = camMain.WorldToScreenPoint(transform.parent.transform.position);
    }

    public void UpdateShield()
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
