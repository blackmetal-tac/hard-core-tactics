using TMPro;
using System;
using UnityEngine;
using DG.Tweening;
using OWS.ObjectPooling;

public class GameManager : MonoBehaviour
{
    private Camera camMain;
    private GameObject executeButton, buttonBorder, crosshair, enemy, actionMask, 
        clickMarker;
    private PlayerController playerController;
    private AIController AIController;
    private UnitManager playerManager, enemyManager;
    private WeaponUI weaponUI;

    public ObjectPool<PoolObject> bulletsPool, missilesPool, amsPool, explosionPool;
    private GameObject projectileOBJ;

    // UI settings
    public float crosshairBars, loadTime, songTitleSpeed;

    // Audio
    private AudioSource audioUI;
    private AudioClip buttonClick;

    // Turn timer
    private static TextMeshProUGUI timer;    
    private float timeValue;
    public float turnTime;
    public bool inAction = false;

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        AIController = GameObject.Find("Enemy").GetComponent<AIController>();
        playerManager = playerController.GetComponentInChildren<UnitManager>();
        enemyManager = AIController.GetComponentInChildren<UnitManager>();

        // Find projectiles and create pools
        projectileOBJ = transform.Find("Projectiles").Find("Bullet").gameObject;
        bulletsPool = new ObjectPool<PoolObject>(projectileOBJ);
        projectileOBJ = transform.Find("Projectiles").Find("Missile").gameObject;
        missilesPool = new ObjectPool<PoolObject>(projectileOBJ);
        projectileOBJ = transform.Find("Projectiles").Find("AMS").gameObject;
        amsPool = new ObjectPool<PoolObject>(projectileOBJ);
        projectileOBJ = transform.Find("Projectiles").Find("Explosion").gameObject;
        explosionPool = new ObjectPool<PoolObject>(projectileOBJ);

        // UI
        weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        clickMarker = GameObject.Find("ClickMarker");
        executeButton = GameObject.Find("ExecuteButton");
        actionMask = executeButton.transform.parent.Find("ActionMask").gameObject;
        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonBorder = executeButton.transform.Find("ButtonBorder").gameObject;
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().ClickButton;
       
        // Turn timer
        timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        timeValue = turnTime;
        timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");

        // Set target
        crosshair = GameObject.Find("Crosshair");
        enemy = GameObject.Find("Enemy");
    }

    void FixedUpdate()
    {
        // Crosshair position
        crosshair.transform.position = camMain.WorldToScreenPoint(enemy.transform.position);
    }

    // Start turn
    public void ExecuteOrder()
    {
        audioUI.PlayOneShot(buttonClick);
        buttonBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.ButtonDelay, () => 
        {
            buttonBorder.transform.DOScaleX(1f, 0.1f);
        });

        inAction = true;

        // Disable buttons
        actionMask.transform.localScale = Vector3.one;

        // Enemy actions ???
        if (!enemyManager.IsDead)
        {
            AIController.SetPath(playerController.PlayerAgent);
            AIController.Move();
        }

        // Player actions
        playerController.Move();

        // Action phase
        this.Progress(turnTime, () => {
            
            timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(timeValue).ToString("ss\\'ff");
            timeValue -= Time.deltaTime;

            // Enemy actions ???
            if (!enemyManager.IsDead)
            {
                enemyManager.StartAction();
            }

            // Player actions ???
            playerManager.StartAction();
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

            // Units actions
            if (!enemyManager.IsDead)
            {
                AIController.EndMove();
            }

            if (!playerManager.IsDead)
            {
                playerController.EndMove();
            }

            // Update player weapon counters
            weaponUI.DecreaseCounter();
        });
    }
}
