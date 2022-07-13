using TMPro;
using System;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    private Camera camMain;
    private GameObject executeButton, buttonBorder, crosshair, enemy, actionMask, 
        clickMarker;
    private PlayerController playerController;
    private AIController AIController;
    private UnitManager playerManager, enemyManager;
    private Crosshair crosshairScr;

    // UI settings
    public float crosshairBars;
    public float loadTime;
    public float songTitleSpeed;

    // Audio
    private AudioSource audioUI;
    private AudioClip buttonClick;

    // Turn timer
    private static TextMeshProUGUI timer;    
    private float timeValue;
    public float turnTime;

    public bool inAction { get; set; }    

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        inAction = false;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        AIController = GameObject.Find("Enemy").GetComponent<AIController>();
        playerManager = playerController.GetComponentInChildren<UnitManager>();
        enemyManager = AIController.GetComponentInChildren<UnitManager>();

        // UI
        clickMarker = GameObject.Find("ClickMarker");
        executeButton = GameObject.Find("ExecuteButton");
        actionMask = executeButton.transform.Find("ActionMask").gameObject;
        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonBorder = executeButton.transform.Find("ButtonBorder").gameObject;
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().clickButton;

        // Turn timer
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");

        // Set target
        crosshair = GameObject.Find("Crosshair");
        crosshairScr = crosshair.GetComponent<Crosshair>();
        enemy = GameObject.Find("Enemy");
    }

    // Update is called once per frame
    void Update()
    {
        // Crosshair position
        crosshair.transform.position = camMain.WorldToScreenPoint(enemy.transform.position);     
    }

    // Start turn
    public void ExecuteOrder()
    {
        audioUI.PlayOneShot(buttonClick);
        buttonBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.buttonDelay, () => 
        {
            buttonBorder.transform.DOScaleX(1f, 0.1f);
        });

        inAction = true;

        // Disable buttons
        actionMask.transform.localScale = Vector3.one;

        // Enemy actions
        if (!enemyManager.isDead)
        {
            AIController.SetPath(playerController.gameObject);
            AIController.Move();
        }

        // Player actions
        playerController.Move();

        // Action phase
        this.Progress(turnTime, () => {
            
            timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
            timeValue -= Time.deltaTime;

            // Enemy actions
            if (!enemyManager.isDead)
            {
                enemyManager.StartAim(playerController.gameObject);
                enemyManager.StartShoot();
            }

            // Player actions
            playerManager.StartAim(AIController.gameObject);
            playerManager.StartShoot();
        });

        // At the end of turn
        this.Wait(turnTime, () =>
        { 
            clickMarker.transform.localScale = Vector3.zero;

            // Enable buttons
            actionMask.transform.localScale = Vector3.zero;
            timeValue = turnTime;
            timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
            inAction = false;

            // Enemy actions
            if (!enemyManager.isDead)
            {
                AIController.EndMove();
            }            

            playerController.EndMove();

            // Update crosshair size after end moving
            crosshairScr.Yoyo();
        });
    }
}
