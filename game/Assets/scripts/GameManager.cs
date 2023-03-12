using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using OWS.ObjectPooling;

public class GameManager : MonoBehaviour
{
    private GameObject _executeBorder, _actionMask, _switchBorder, _switchMask, _enemy, _clickMarker, _projectileOBJ;    
    private WeaponUI _weaponUI;
    private CoreButton _coreButton;    

    public ObjectPool<PoolObject> BulletsPool, MissilesPool, AmsPool, ExplosionPool;

    // Control keys    
    private PlayerInput _playerInput;
    private InputAction _slow, _pause, _switchUnit;
    private bool _paused, _slowed;

    // UI settings
    public float CrosshairBars, LoadTime;

    // Audio
    private AudioSource _audioUI;
    private AudioClip _buttonClick;

    // Turn timer
    private static TextMeshProUGUI _timer, _switchButton;    
    public float TimeValue, TurnTime;

    [HideInInspector] public bool InAction, UpdateTargets;

    [System.Serializable]
    public class Formation
    {
        public string FormationName;
        public Vector3[] Positions;
    }
    public List<Formation> UnitsFormations = new List<Formation>(); 
    [HideInInspector] public SquadManager PlayerSquad, EnemySquad;

    void OnEnable()
    {
        _slow = _playerInput.UI.Slow;
        _slow.Enable();

        _pause = _playerInput.UI.Pause;
        _pause.Enable();

        _switchUnit = _playerInput.Player.Switch;
        _switchUnit.Enable();
    }

    void OnDisable()
    {
        _slow.Disable();
        _pause.Disable();
        _switchUnit.Disable();
    }

    void Awake()
    {
        _playerInput = new PlayerInput();
    }

    // Start is called before the first frame update
    void Start()
    {
        DOTween.SetTweensCapacity(800, 50);
        PlayerSquad = GameObject.Find("PlayerSquad").GetComponent<SquadManager>();
        EnemySquad = GameObject.Find("EnemySquad").GetComponent<SquadManager>();        

        // Find projectiles and create pools
        _projectileOBJ = transform.Find("Projectiles").Find("Bullet").gameObject;
        BulletsPool = new ObjectPool<PoolObject>(_projectileOBJ);
        _projectileOBJ = transform.Find("Projectiles").Find("Missile").gameObject;
        MissilesPool = new ObjectPool<PoolObject>(_projectileOBJ);
        _projectileOBJ = transform.Find("Projectiles").Find("AMS").gameObject;
        AmsPool = new ObjectPool<PoolObject>(_projectileOBJ);
        _projectileOBJ = transform.Find("Projectiles").Find("Explosion").gameObject;
        ExplosionPool = new ObjectPool<PoolObject>(_projectileOBJ);

        // UI
        _weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        _clickMarker = GameObject.Find("ClickMarker");        
        _actionMask = GameObject.Find("ExecuteButton").transform.parent.Find("ActionMask").gameObject;
        _executeBorder = GameObject.Find("ExecuteButton").transform.Find("ButtonBorder").gameObject;
        _switchButton = GameObject.Find("SwitchButton").GetComponentInChildren<TextMeshProUGUI>();
        _switchMask = GameObject.Find("SwitchButton").transform.parent.Find("ActionMask").gameObject;
        _switchBorder = GameObject.Find("SwitchButton").transform.Find("ButtonBorder").gameObject;
        _audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();        
        _buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().ClickButton;
        _coreButton = GameObject.Find("CoreButton").GetComponent<CoreButton>();
       
        // Turn timer
        _timer = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        TimeValue = TurnTime;
        _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");

        // Set target ???        
        _enemy = GameObject.Find("Enemy").transform.GetChild(0).gameObject;
    }
    
    void Update()
    {
        // Stop game
        if (_pause.WasPressedThisFrame())
        {        
            _slowed = false;    
            _paused = !_paused;
            if (_paused)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        // Slow game
        if (_slow.WasPressedThisFrame())
        {            
            _paused = false;
            _slowed = !_slowed;
            if (_slowed)
            {
                Time.timeScale = 0.5f;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        if (Time.time > LoadTime && !InAction && _switchUnit.WasPressedThisFrame() && PlayerSquad.SwitchCooldown <= 0)
        {
            SwitchUnit();
        }

        // Action phase
        if (InAction)
        {
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");
            TimeValue -= Time.deltaTime;

            // Player actions
            foreach (AIController controller in PlayerSquad.AIControllers)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.StartAction();
                }
            }

            // Enemy actions
            foreach (AIController controller in EnemySquad.AIControllers)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.StartAction();
                }
            }  
        }
    }

    // Start turn
    public void ExecuteOrder()
    {
        _audioUI.PlayOneShot(_buttonClick);
        _executeBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.ButtonDelay, () => 
        {
            _executeBorder.transform.DOScaleX(1f, 0.1f);
        });

        InAction = true;

        // Player actions        
        PlayerSquad.ApplyPositions();        
        foreach (AIController controller in PlayerSquad.AIControllers)
        {       
            controller.Move();
        }

        // Enemy actions 
        EnemySquad.ApplyPositions();       
        foreach (AIController controller in EnemySquad.AIControllers)
        {
            controller.Move();
        }   

        // Disable buttons
        _actionMask.transform.localScale = Vector3.one;
        _switchMask.transform.localScale = Vector3.one;
        UpdateSwitchCounter();

        // At the end of turn
        this.Wait(TurnTime, () =>
        { 
            _clickMarker.transform.localScale = Vector3.zero;
            UpdateTargets = false;

            // Enable buttons
            _actionMask.transform.localScale = Vector3.zero;        

            TimeValue = TurnTime;
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");
            InAction = false;

            // Player actions end
            foreach (AIController controller in PlayerSquad.AIControllers)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.EndMove();
                }
            }

            // Enemy actions ens
            foreach (AIController controller in EnemySquad.AIControllers)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.EndMove();
                }
            }
            
            // Update player weapon counters
            _weaponUI.DecreaseCounter();
            PlayerSquad.SwitchCooldown --;
            EnemySquad.SwitchCooldown --;
            UpdateSwitchCounter();
        });
    }

    public void SwitchUnit()
    {
        PlayerSquad.SwitchUnit();

        _audioUI.PlayOneShot(_buttonClick);
        _switchBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.ButtonDelay, () => 
        {
            _switchBorder.transform.DOScaleX(1f, 0.1f);
        });       
    }

    private void UpdateSwitchCounter()
    {
        if (PlayerSquad.SwitchCooldown > 0)
        {
            if (PlayerSquad.SwitchCooldown == 1)
            {
                _switchButton.text = PlayerSquad.SwitchCooldown + "<br> trn";
            }
            else
            {
                _switchButton.text = PlayerSquad.SwitchCooldown + "<br> trns";
            }            
        }
        else
        {
            _switchButton.text = "Switch";
            if (!InAction)
            {
                _switchMask.transform.localScale = Vector3.zero;
            }            
        }
    }
}
