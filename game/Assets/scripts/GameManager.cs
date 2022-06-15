using TMPro;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static TextMeshProUGUI timer;    
    private float timeValue;

    public float turnTime = 3f;

    public bool inAction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        inAction = false;      

        // Turn timer
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = timeValue.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        // Timer display      
        timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
        if (inAction && timeValue > 0)
        {
            timeValue -= Time.deltaTime;
        }
        else
        {
            inAction = false;
            timeValue = turnTime;
        }
    }

}
