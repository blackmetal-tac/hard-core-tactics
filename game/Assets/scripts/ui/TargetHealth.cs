using UnityEngine;
using UnityEngine.UI;

public class TargetHealth : MonoBehaviour
{
    public GameObject unit;
    private Image healthImage, damageImage;
    private UnitStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = unit.GetComponent<UnitStats>();
        healthImage = transform.Find("Health").GetComponent<Image>();
        damageImage = transform.Find("Damage").GetComponent<Image>();
        damageImage.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        healthImage.fillAmount = stats.HP;

        //Damage animation
        stats.shrinkTimer -= Time.deltaTime;
        if (stats.shrinkTimer < 0) 
        {
            float shrinkSpeed = 1f;
            if (healthImage.fillAmount < damageImage.fillAmount) 
            {
                damageImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }
}
