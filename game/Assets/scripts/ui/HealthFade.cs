using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthFade : MonoBehaviour
{
    private static Image barImage;

    // Start is called before the first frame update
    void Start()
    {
        barImage = transform.Find("TargetHealth").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        barImage.fillAmount = AIController.HP;        
    }
}
