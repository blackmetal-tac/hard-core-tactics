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

    public ObjectPool<PoolObject> bulletsPool;

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

        inAction = true;

        // Disable buttons
        actionMask.transform.localScale = Vector3.one;

        // Enemy actions
        if (!enemyManager.isDead)
        {
            AIController.SetPath();
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
                AIController.Aim();
                enemyManager.FireBurst(enemyManager.firePoint, bulletsPool, enemyManager.projectile);
            }

            // Player actions
            playerController.Aim();
            playerManager.FireBurst(playerManager.firePoint, bulletsPool, playerManager.projectile);            
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
