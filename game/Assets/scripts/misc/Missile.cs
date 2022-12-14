using UnityEngine;
using OWS.ObjectPooling;

public class Missile : MonoBehaviour
{
    [HideInInspector] public float Damage;
    [HideInInspector] public float Speed;
    [HideInInspector] public Vector3 Target;
    [HideInInspector] public Rigidbody MissileBody;
    [HideInInspector] public Collider MissileCollider;
    [HideInInspector] public bool Homing;
    [HideInInspector] public GameObject HomingTarget;
    private GameManager _gameManager;
    private PoolObject _poolObject;
    private readonly float _spread = 0.5f, _delay = 0.1f, _baseTimer = 0.5f;
    private float _timer, _lastCheck;
    private Vector3 _spreadVector;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _poolObject = GetComponentInParent<PoolObject>();
        MissileBody = GetComponent<Rigidbody>();
        MissileCollider = GetComponent<Collider>();
        _timer = _baseTimer;
		
		// Look at target
        _spreadVector = new(
            Random.Range(-_spread, _spread) + Target.x - _poolObject.transform.position.x,
            Random.Range(-_spread / 4, _spread / 4) + Target.y - _poolObject.transform.position.y,
            Random.Range(-_spread, _spread) + Target.z - _poolObject.transform.position.z);
    }

    void FixedUpdate() 
    {
        if (_poolObject.transform.localScale != Vector3.zero && !Homing)
        {
            MoveTowards();
        }
        else if (_poolObject.transform.localScale != Vector3.zero && Homing)
        {
            FollowTarget();
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
            if (collider.name == "BulletMesh")
            Debug.Log(collider.name);
        }
  
    }

    public void Explode()
    {
        _timer = _baseTimer;            
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
            Vector3 direction = Target - _poolObject.transform.position;
            _poolObject.transform.rotation = Quaternion.RotateTowards(_poolObject.transform.rotation,
                Quaternion.LookRotation(direction + _spreadVector), Time.time * 0.1f);
        }
 
        _poolObject.transform.position += Speed * Time.fixedDeltaTime * _poolObject.transform.forward;
        _timer -= Time.fixedDeltaTime;
    }

    public void FollowTarget()
    {
        if (Time.time > _lastCheck + _delay)
        {
            CalculateSpread();
            _lastCheck = Time.time;            
        }

        if (_timer > 0 && _timer < _baseTimer)
        {
            _poolObject.transform.rotation = Quaternion.RotateTowards(_poolObject.transform.rotation,
                Quaternion.LookRotation(_poolObject.transform.forward + _spreadVector), Time.time * 0.1f);
        }
        else if (_timer > -_baseTimer)
        {
            Vector3 direction = HomingTarget.transform.position - _poolObject.transform.position;
            _poolObject.transform.rotation = Quaternion.RotateTowards(_poolObject.transform.rotation,
                Quaternion.LookRotation(direction + _spreadVector), Time.time * 1f);
        }

        _poolObject.transform.position += Speed * Time.fixedDeltaTime * _poolObject.transform.forward;
        _timer -= Time.fixedDeltaTime;
    }

    public void CalculateSpread()
    {
        _spreadVector = new(
            Random.Range(-_spread, _spread),
            Random.Range(-_spread / 4, _spread / 4),
            Random.Range(-_spread, _spread));
    }
}
