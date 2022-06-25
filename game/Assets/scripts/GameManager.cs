using TMPro;
using System;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private Camera camMain;
    private GameObject executeButton, buttonFrame, playerUI, crosshair, enemy, player;
    private PlayerController playerController;
    private AIController AIController;
    private UnitManager unitManager;
    private Crosshair crosshairScr;

    // UI settings
    public float crosshairBars;
    public float loadTime;

    // Audio
    private AudioSource audioUI;
    private AudioClip buttonClick;

    // Turn timer
    private static TextMeshProUGUI timer;    
    private float timeValue;
    public float turnTime = 3f;

    public bool inAction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        inAction = false;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        AIController = GameObject.Find("Enemy").GetComponent<AIController>();
        unitManager = playerController.GetComponentInChildren<UnitManager>();

        // UI
        executeButton = GameObject.Find("ExecuteButton");
        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonFrame = executeButton.transform.GetChild(1).gameObject;
        playerUI = GameObject.Find("PlayerUI");
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().clickButton;

        // Turn timer
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = timeValue.ToString();

        // Set target
        crosshair = GameObject.Find("Crosshair");
        crosshairScr = crosshair.GetComponent<Crosshair>();
        enemy = GameObject.Find("Enemy");
        player = GameObject.Find("Player");          
    }

    // Update is called once per frame
    void Update()
    {
        // Crosshair position
        crosshair.transform.position = camMain.WorldToScreenPoint(enemy.transform.position);
        playerUI.transform.position = camMain.WorldToScreenPoint(player.transform.position);

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

    // Start turn
    public void ExecuteOrder()
    {
        audioUI.PlayOneShot(buttonClick);
        buttonFrame.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.buttonDelay, () => 
        {
            buttonFrame.transform.DOScaleX(1f, 0.1f);
        });

        this.Wait(turnTime, () =>
        {
            playerController.unitManager.moveSpeed = 0.1f;
            crosshairScr.Yoyo();             
        });

        playerController.playerAgent.speed = unitManager.moveSpeed;
        inAction = true;
        AIController.SetPath();
    }
}
