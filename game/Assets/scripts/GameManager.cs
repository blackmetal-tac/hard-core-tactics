using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using OWS.ObjectPooling;

public class GameManager : MonoBehaviour
{
    private GameObject _executeBorder, _actionMask, _switchBorder, _switchMask, _enemy, _clickMarker, _projectileOBJ,
        _playerSquad, _enemySquad;    
    [HideInInspector] public List<AIController> AIControllersPlayer = new List<AIController>(), AIControllersEnemy = new List<AIController>();    
    private WeaponUI _weaponUI;
    private CoreButton _coreButton;
    private CameraMovement _cameraMov;
    public int CurrentUnit;

    public ObjectPool<PoolObject> BulletsPool, MissilesPool, AmsPool, ExplosionPool;

    // Control keys    
    private PlayerInput _playerInput;
    private InputAction _pause, _switchUnit;
    private bool _paused;

    // UI settings
    public float CrosshairBars, LoadTime;

    // Audio
    private AudioSource _audioUI;
    private AudioClip _buttonClick;

    // Turn timer
    private static TextMeshProUGUI _timer;    
    public float TimeValue, TurnTime;

    [HideInInspector] public bool InAction = false;

    [System.Serializable]
    public class Formation
    {
        public string FormationName;
        public Vector3[] Positions;
    }
    public List<Formation> UnitsFormations = new List<Formation>(); 

    void OnEnable()
    {
        _pause = _playerInput.UI.Pause;
        _pause.Enable();

        _switchUnit = _playerInput.Player.Switch;
        _switchUnit.Enable();
    }

    void OnDisable()
    {
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
        _playerSquad = GameObject.Find("PlayerSquad");
        _enemySquad = GameObject.Find("EnemySquad");
        _cameraMov = Camera.main.GetComponent<CameraMovement>();
        
        for (int i = 0; i < _playerSquad.transform.childCount; i++)
        {            
            AIControllersPlayer.Add(_playerSquad.transform.GetChild(i).GetComponent<AIController>());
        }

        for (int i = 0; i < _enemySquad.transform.childCount; i++)
        {            
            AIControllersEnemy.Add(_enemySquad.transform.GetChild(i).GetComponent<AIController>());
        }

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

        if (Time.time > LoadTime && !InAction && _switchUnit.WasPressedThisFrame())
        {
            SwitchUnit();
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

        // Disable buttons
        _actionMask.transform.localScale = Vector3.one;
        _switchMask.transform.localScale = Vector3.one;

        // Player actions        
        //ApplyPositions();
        foreach (AIController controller in AIControllersPlayer)
        {
            controller.Move();
        }

        // Enemy actions
        foreach (AIController controller in AIControllersEnemy)
        {
            controller.Move();
        }      

        // Action phase
        this.Progress(TurnTime, () => {
            
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");
            TimeValue -= Time.deltaTime;

            // Player actions
            foreach (AIController controller in AIControllersPlayer)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.StartAction();
                }
            }

            // Enemy actions
            foreach (AIController controller in AIControllersEnemy)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.StartAction();
                }
            }       
        });

        // At the end of turn
        this.Wait(TurnTime, () =>
        { 
            _clickMarker.transform.localScale = Vector3.zero;

            // Enable buttons
            _actionMask.transform.localScale = Vector3.zero;
            _switchMask.transform.localScale = Vector3.zero;

            TimeValue = TurnTime;
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");
            InAction = false;

            // Player actions end
            foreach (AIController controller in AIControllersPlayer)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.EndMove();
                }
            }

            // Enemy actions ens
            foreach (AIController controller in AIControllersEnemy)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.EndMove();
                }
            }
            
            // Update player weapon counters
            _weaponUI.DecreaseCounter();
        });
    }

    public void SwitchUnit()
    {
        AIControllersPlayer[CurrentUnit].ClearPath();
        if (CurrentUnit < AIControllersPlayer.Count - 1)
        {
            CurrentUnit++;
            AIControllersPlayer[CurrentUnit - 1].UnitManagerP.ShrinkBar.ToggleUI();
            AIControllersPlayer[CurrentUnit].UnitManagerP.ShrinkBar.ToggleUI();            
        }
        else
        {
            CurrentUnit = 0;
            AIControllersPlayer[AIControllersPlayer.Count - 1].UnitManagerP.ShrinkBar.ToggleUI();
            AIControllersPlayer[CurrentUnit].UnitManagerP.ShrinkBar.ToggleUI();            
        }        

        // Move camera to next unit
        _cameraMov.Destination = AIControllersPlayer[CurrentUnit].gameObject;

        // Update UI for new unit
        _weaponUI.UpdateUI(AIControllersPlayer[CurrentUnit].name);

        _audioUI.PlayOneShot(_buttonClick);
        _switchBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.ButtonDelay, () => 
        {
            _switchBorder.transform.DOScaleX(1f, 0.1f);
        });       
    }

    private void ApplyPositions()
    {
        if (CurrentUnit < AIControllersPlayer.Count - 1)
        {
            SetControllers(CurrentUnit);
        }
        else
        {            
            SetControllers(AIControllersPlayer.Count);  
        }   
    }

    private void SetControllers(int index)
    {
        AIControllersPlayer[index - 1].name = AIControllersPlayer[CurrentUnit].name;
        AIControllersPlayer[CurrentUnit].name = "Player";
        AIControllersPlayer[index - 1].UpdateManager();
        AIControllersPlayer[CurrentUnit].UpdateManager();
        AIControllersPlayer[index - 1].SetUnitsPos();
        AIControllersPlayer[CurrentUnit].SetUnitsPos();

        foreach (AIController controller in AIControllersPlayer)
        {
            controller.SetTargets("EnemySquad"); 
        }

        foreach (AIController controller in AIControllersEnemy)
        {
            controller.SetTargets("PlayerSquad"); 
        }
    }
}
