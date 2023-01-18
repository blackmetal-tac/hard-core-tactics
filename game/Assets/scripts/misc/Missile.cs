using UnityEngine;
using OWS.ObjectPooling;

public class Missile : MonoBehaviour
{
    [HideInInspector] public float Damage, Speed;
    [HideInInspector] public UnitManager Target;
    [HideInInspector] public Vector3 SpreadVector;
    [HideInInspector] public Rigidbody MissileBody;
    [HideInInspector] public Collider MissileCollider;
    [HideInInspector] public bool Homing;    
    private GameManager _gameManager;
    private PoolObject _poolObject;
    private readonly float _spread = 0.5f, _delay = 0.1f, _baseTimer = 0.5f;
    private float _timer, _lastCheck;
    private Vector3 _spreadVector;
    private bool _oneTime;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _poolObject = GetComponentInParent<PoolObject>();
        MissileBody = GetComponent<Rigidbody>();
        MissileCollider = GetComponent<Collider>();
        _timer = _baseTimer;
    }

    void Update() 
    {
        if (_poolObject.transform.localScale != Vector3.zero && !Homing)
        {
            MoveTowards();
            if (_gameManager.InAction)
            {
                Target.MissileLockTimer = 0.5f;
            }            
        }
        else if (_poolObject.transform.localScale != Vector3.zero && Homing)
        {
            FollowTarget();
            if (_gameManager.InAction)
            {
                Target.MissileLockTimer = 0.5f;
            }  
        }
    }

    // Missile collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(Damage * 2);
        }

        if (collider.name != "ColliderAMS")
        {
            Explode();                    
        }
    }

    public void Explode()
    {
        _timer = _baseTimer;
        _oneTime = false;
        MissileBody.velocity = Vector3.zero;
        MissileCollider.enabled = false;
        _gameManager.ExplosionPool.PullGameObject(transform.position, 1f, Damage / 2);
        _poolObject.ReturnToPool();
    }

    public void MoveTowards()
    {        
        if (Time.time > _lastCheck + _delay && _timer > 0)
        {
            CalculateSpread();
            _lastCheck = Time.time;            
        }

        if (_timer > 0)
        {
            Vector3 direction = (Target.transform.position + SpreadVector) - _poolObject.transform.position;
            _poolObject.transform.rotation = Quaternion.RotateTowards(_poolObject.transform.rotation,
                Quaternion.LookRotation(direction + _spreadVector), Time.time * 0.1f);
        }
 
        _poolObject.transform.position += Speed * Time.deltaTime * _poolObject.transform.forward;
        _timer -= Time.deltaTime;
    }

    public void FollowTarget()
    {
        if (Time.time > _lastCheck + _delay)
        {
            CalculateSpread();
            _lastCheck = Time.time;            
        }

        if (_timer > 0)
        {            
            _poolObject.transform.rotation = Quaternion.RotateTowards(_poolObject.transform.rotation,
                Quaternion.LookRotation(_poolObject.transform.forward + _spreadVector / 2), Time.time * 0.1f);
        }
        else if (_timer < 0 && _timer > -_baseTimer)
        {
            if (!_oneTime)
            {
                Speed += Speed / 2;
                _oneTime = true;               
            }

            Vector3 direction = Target.transform.position - _poolObject.transform.position;
            _poolObject.transform.rotation = Quaternion.RotateTowards(_poolObject.transform.rotation,
                Quaternion.LookRotation(direction + _spreadVector / 2), Time.time * 1f);
        }        
        _poolObject.transform.position += Speed * Time.deltaTime * _poolObject.transform.forward;
        _timer -= Time.deltaTime;
    }

    private void CalculateSpread()
    {
        _spreadVector = new(
            Random.Range(-_spread, _spread),
            Random.Range(-_spread / 4, _spread / 4),
            Random.Range(-_spread, _spread));
    }
}
