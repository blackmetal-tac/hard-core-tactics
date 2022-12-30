using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Shield : MonoBehaviour
{
    [HideInInspector] public float HP, Regeneration, Heat, Scale, ShrinkTimer, ShieldIntencity, IntencityBuffer;
    [HideInInspector] public Material Material;
    [HideInInspector] public string ShieldID;
    [HideInInspector] public int DownTimer;    
    [SerializeField] private float _size = 0.9f;
    private Collider _shieldCollider;
    private GameManager _gameManager;
    private bool _loading;
    [ColorUsage(true, true)] public Color ShieldColor;

    [System.Serializable]
    public class ShieldModes
    {
        public string ModeName;
        public float Regen;
        public float Heat;
        public int Intencity;
    }
    public List<ShieldModes> shieldModes;
	
    void Start()
    {        
        _shieldCollider = GetComponent<Collider>();
        Material = GetComponentInChildren<MeshRenderer>().material;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Enable shield
        _loading = true;
        transform.DOScale(_size, _gameManager.LoadTime).SetEase(Ease.OutBack);
        this.Wait(_gameManager.LoadTime, () => {
            _loading = false;
        });        
    }

    void Update()
    {  
        if (!_loading)
        {
            transform.localScale = (_size + Scale) * Vector3.one;
        }
        
        Material.SetFloat("_Alpha", (0.3f + ShieldIntencity / 10) * HP);                

        if (HP > 0)
        {
            _shieldCollider.enabled = true;
        }
    }

    public void TakeDamage(float damage)
    {
        // Reset HP bar damage animation
        ShrinkTimer = 0.5f;
        if (HP > 0)
        {
            HP -= damage;            
        }

        // Shield down
        if (HP <= 0)
        {
            _shieldCollider.enabled = false;
        }
    }

    public void Regenerate()
    {
        // Shield regeneration
        if (HP < 1)
        {
            if (Regeneration == 0)  
            {
                HP += Time.deltaTime * (shieldModes[2].Regen);                
            }      
            else 
            {
                HP += Time.deltaTime * (Regeneration);
            }  
        }
    }

    public void TurnOnOff()
    { 
        if (Regeneration == 0 && _gameManager.InAction)
        {
            ShieldIntencity = IntencityBuffer;
            Material.SetColor("_Color", ShieldColor * ShieldIntencity);

            _loading = true;
            transform.DOScale(0, _gameManager.LoadTime);
        }
        else if (_gameManager.InAction)
        {   
            ShieldIntencity = IntencityBuffer;
            Material.SetColor("_Color", ShieldColor * ShieldIntencity);

            transform.DOScale(_size, _gameManager.LoadTime).SetEase(Ease.OutBack);
            this.Wait(_gameManager.LoadTime, () => {
                _loading = false;
            });
        }
    }

    public void ChangeMode(ShieldModes shieldMode)
    {
        Regeneration = shieldMode.Regen;
        Heat = shieldMode.Heat;
        IntencityBuffer = shieldMode.Intencity;
    }
}
