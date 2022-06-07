using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private Image healthImage, damageImage; 
    public static float shrinkTimer;

    private CharacterStats stats;

    // Start is called before the first frame update
    void Start()
    {
        stats = GameObject.Find("Player").GetComponent<CharacterStats>();
        healthImage = transform.Find("PlayerHealth").GetComponent<Image>();
        damageImage = transform.Find("PlayerDamage").GetComponent<Image>();
        damageImage.fillAmount = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        healthImage.fillAmount = stats.HP;

        //Damage animation
        shrinkTimer -= Time.deltaTime;
        if (shrinkTimer < 0) 
        {
            float shrinkSpeed = 1f;
            if (healthImage.fillAmount < damageImage.fillAmount) 
            {
                damageImage.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }
    }
}
