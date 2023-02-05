using TMPro;
using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using DG.Tweening;
using OWS.ObjectPooling;

public class GameManager : MonoBehaviour
{
    private GameObject _executeButton, _buttonBorder, _enemy, _actionMask, 
        _clickMarker, _projectileOBJ, _playerSquad, _enemySquad;
    private PlayerController _playerController;
    private List<AIController> _AIControllersPlayer = new List<AIController>(), _AIControllersEnemy = new List<AIController>();    
    private WeaponUI _weaponUI;
    private CoreButton _coreButton;

    public ObjectPool<PoolObject> BulletsPool, MissilesPool, AmsPool, ExplosionPool;

    // Control keys    
    private PlayerInput _playerInput;
    private InputAction _pause;
    private bool _paused;

    // UI settings
    public float CrosshairBars, LoadTime;

    // Audio
    private AudioSource _audioUI;
    private AudioClip _buttonClick;

    // Turn timer
    private static TextMeshProUGUI _timer;    
    public float TimeValue;
    public float TurnTime;
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
    }

    void OnDisable()
    {
        _pause.Disable();
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
        _playerController = _playerSquad.GetComponentInChildren<PlayerController>(); 
        
        for (int i = 1; i < _playerSquad.transform.childCount; i++)
        {            
            _AIControllersPlayer.Add(_playerSquad.transform.GetChild(i).GetComponent<AIController>());
        }

        for (int i = 0; i < _enemySquad.transform.childCount; i++)
        {            
            _AIControllersEnemy.Add(_enemySquad.transform.GetChild(i).GetComponent<AIController>());
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
        _executeButton = GameObject.Find("ExecuteButton");
        _actionMask = _executeButton.transform.parent.Find("ActionMask").gameObject;
        _audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        _buttonBorder = _executeButton.transform.Find("ButtonBorder").gameObject;
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
        if (_pause.triggered)
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
    }

    // Start turn
    public void ExecuteOrder()
    {
        _audioUI.PlayOneShot(_buttonClick);
        _buttonBorder.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.ButtonDelay, () => 
        {
            _buttonBorder.transform.DOScaleX(1f, 0.1f);
        });

        InAction = true;

        // Disable buttons
        _actionMask.transform.localScale = Vector3.one;

        // Player actions
        _playerController.Move();
        foreach (AIController controller in _AIControllersPlayer)
        {
            controller.Move();
        }

        // Enemy actions
        foreach (AIController controller in _AIControllersEnemy)
        {
            controller.Move();
        }      

        // Action phase
        this.Progress(TurnTime, () => {
            
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");
            TimeValue -= Time.deltaTime;

            // Player actions
            if (!_playerController.UnitManagerP.IsDead)
            {
                _playerController.UnitManagerP.StartAction();
            }

            foreach (AIController controller in _AIControllersPlayer)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.StartAction();
                }
            }

            // Enemy actions
            foreach (AIController controller in _AIControllersEnemy)
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
            TimeValue = TurnTime;
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(TimeValue).ToString("ss\\'ff");
            InAction = false;

            // Player actions end
            if (!_playerController.UnitManagerP.IsDead)
            {
                _playerController.EndMove();
            }

            foreach (AIController controller in _AIControllersPlayer)
            {
                if (!controller.UnitManagerP.IsDead)
                {
                    controller.EndMove();
                }
            }

            // Enemy actions ens
            foreach (AIController controller in _AIControllersEnemy)
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
}
