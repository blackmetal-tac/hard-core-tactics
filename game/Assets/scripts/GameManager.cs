using TMPro;
using System;
using UnityEngine;
using DG.Tweening;
using OWS.ObjectPooling;

public class GameManager : MonoBehaviour
{
    private GameObject _executeButton, _buttonBorder, _enemy, _actionMask, 
        _clickMarker, _projectileOBJ;
    private PlayerController _playerController;
    private AIController _AIController;
    private UnitManager _playerManager, _enemyManager;
    private WeaponUI _weaponUI;
    private CoreButton _coreButton;

    public ObjectPool<PoolObject> BulletsPool, MissilesPool, AmsPool, ExplosionPool;

    // UI settings
    public float CrosshairBars, LoadTime;

    // Audio
    private AudioSource _audioUI;
    private AudioClip _buttonClick;

    // Turn timer
    private static TextMeshProUGUI _timer;    
    private float _timeValue;
    public float TurnTime;
    [HideInInspector] public bool InAction = false;

    // Start is called before the first frame update
    void Start()
    {
        DOTween.SetTweensCapacity(500, 50);
        _playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        _AIController = GameObject.Find("Enemy").GetComponent<AIController>();
        _playerManager = _playerController.GetComponentInChildren<UnitManager>();
        _enemyManager = _AIController.GetComponentInChildren<UnitManager>();

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
        _timeValue = TurnTime;
        _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(_timeValue).ToString("ss\\'ff");

        // Set target ???        
        _enemy = GameObject.Find("Enemy").transform.GetChild(0).gameObject;
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

        // Enemy actions ???
        if (!_enemyManager.IsDead)
        {
            _AIController.SetPath(_playerController.PlayerAgent);            
            _AIController.Move();
        }

        // Player actions
        if (!_playerManager.IsDead)
        {
            _playerController.Move();            
        }        

        // Action phase
        this.Progress(TurnTime, () => {
            
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(_timeValue).ToString("ss\\'ff");
            _timeValue -= Time.deltaTime;

            // Enemy actions ???
            if (!_enemyManager.IsDead)
            {
                _enemyManager.StartAction();                
            }

            // Player actions ???
            if (!_playerManager.IsDead)
            {
                _playerManager.StartAction(); 
            }                       
        });

        // At the end of turn
        this.Wait(TurnTime, () =>
        { 
            _clickMarker.transform.localScale = Vector3.zero;

            // Enable buttons
            _actionMask.transform.localScale = Vector3.zero;
            _timeValue = TurnTime;
            _timer.text = "<mspace=0.6em>" + TimeSpan.FromSeconds(_timeValue).ToString("ss\\'ff");
            InAction = false;

            // Units actions end
            if (!_enemyManager.IsDead)
            {
                _AIController.EndMove();                
            }

            if (!_playerManager.IsDead)
            {
                _playerController.EndMove();                
            }

            // Update player weapon counters
            _weaponUI.DecreaseCounter();
        });
    }
}
