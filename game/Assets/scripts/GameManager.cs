using TMPro;
using System;
using UnityEngine;
using DG.Tweening;
using OWS.ObjectPooling;

public class GameManager : MonoBehaviour
{
    private Camera camMain;
    private GameObject executeButton, buttonFrame, crosshair, enemy, actionMask, 
        clickMarker;
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
    public float turnTime;

    public bool inAction { get; set; }

    public ObjectPool<PoolObject> bulletsPool;

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        inAction = false;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        AIController = GameObject.Find("Enemy").GetComponent<AIController>();
        unitManager = playerController.GetComponentInChildren<UnitManager>();        

        // UI
        clickMarker = GameObject.Find("ClickMarker");
        executeButton = GameObject.Find("ExecuteButton");
        actionMask = GameObject.Find("ActionMask");
        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonFrame = executeButton.transform.GetChild(1).gameObject;
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().clickButton;

        // Turn timer
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");

        // Set target
        crosshair = GameObject.Find("Crosshair");
        crosshairScr = crosshair.GetComponent<Crosshair>();
        enemy = GameObject.Find("Enemy");

        bulletsPool = new ObjectPool<PoolObject>(playerController.projectile, 15);
    }

    // Update is called once per frame
    void Update()
    {
        // Crosshair position
        crosshair.transform.position = camMain.WorldToScreenPoint(enemy.transform.position);

        if (playerController.playerAgent.hasPath)
        {
            clickMarker.transform.Rotate(new Vector3(0, 0, 50 * -Time.deltaTime));
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

        // At the end of turn
        this.Wait(turnTime, () =>
        {
            //playerController.unitManager.moveSpeed = 0.1f;
            crosshairScr.Yoyo();
            clickMarker.transform.localScale = Vector3.zero;

            // Enable buttons
            actionMask.transform.localScale = Vector3.zero;
            timeValue = turnTime;
            timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
            inAction = false;
        });

        playerController.playerAgent.speed = unitManager.moveSpeed;
        inAction = true;
        AIController.SetPath();
        
        // Action phase
        this.Progress(turnTime, () => {

            // Disable buttons
            actionMask.transform.localScale = Vector3.one;

            timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
            timeValue -= Time.deltaTime;

            unitManager.aiming.StartAim(unitManager);
        });
    }
}
