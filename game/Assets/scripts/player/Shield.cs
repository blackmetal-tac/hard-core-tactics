using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Shield : MonoBehaviour
{
    [HideInInspector] public float HP, Regeneration, Scale, ShrinkTimer;
    [HideInInspector] public Material Material;
    [HideInInspector] public string ShieldID;
    [SerializeField] private float _size = 0.9f;
    private Collider _shieldCollider;
    private GameManager _gameManager;
    private bool _loading;

    [System.Serializable]
    public class ShieldModes
    {
        public string ModeName;
        public int Regen;
    }
    public List<ShieldModes> shieldModes;
	
    void Start()
    {        
        _shieldCollider = GetComponent<Collider>();
        Material = GetComponentInChildren<MeshRenderer>().sharedMaterial;
        _gameManager = _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        _loading = true;
        this.Progress(_gameManager.LoadTime, () => {
            transform.DOScale(_size, _gameManager.LoadTime).SetLoops(1, LoopType.Yoyo);            
        });

        this.Wait(_gameManager.LoadTime, () => {
            _loading = false;
        });
    }

    void FixedUpdate()
    {        
        if (!_loading)
        {
            transform.localScale = (_size + Scale) * Vector3.one;
        }
        
        Material.SetFloat("_Alpha", 0.3f * HP);

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
            HP += Time.deltaTime * Regeneration;            
        }
    }

    public void TurnOnOff()
    {
        if (Regeneration == 0 && _gameManager.InAction)
        {
            _loading = true;
            transform.DOScale(0, _gameManager.LoadTime);
        }
        else if (transform.lossyScale == Vector3.zero && _gameManager.InAction)
        {
            transform.DOScale(_size, _gameManager.LoadTime).SetLoops(1, LoopType.Yoyo);
            this.Wait(_gameManager.LoadTime, () => {
                _loading = false;
            });
        }
    }
}
