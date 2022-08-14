using UnityEngine;
using OWS.ObjectPooling;

public class Missile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public Vector3 target;
    [HideInInspector] public Rigidbody missileBody;
    [HideInInspector] public Collider missileCollider;
    [HideInInspector] public bool homing;
    [HideInInspector] public GameObject homingTarget;
    private PoolObject poolObject;
    private readonly float spread = 1f, delay = 1f, baseTimer = 0.7f;
    private float timer, lastCheck;
    private Vector3 spreadVector;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponentInParent<PoolObject>();
        missileBody = GetComponent<Rigidbody>();
        missileCollider = GetComponent<Collider>();
        timer = baseTimer;

        spreadVector = new(
            Random.Range(-spread, spread) + target.x - poolObject.transform.position.x,
            Random.Range(-spread / 4, spread / 4) + target.y - poolObject.transform.position.y,
            Random.Range(-spread, spread) + target.z - poolObject.transform.position.z);
    }

    void FixedUpdate() 
    {
        if (poolObject.transform.localScale != Vector3.zero && !homing)
        {
            MoveTowards();
        }
        else if (poolObject.transform.localScale != Vector3.zero && homing)
        {
            FollowTarget();
        }
    }

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);
        }

        if (collider.name != "ColliderAMS")
        {    
            timer = baseTimer;
            missileBody.velocity = Vector3.zero;
            missileCollider.enabled = false;
            poolObject.ReturnToPool();
        }
    }

    public void MoveTowards()
    {        
        if (Time.time > lastCheck + delay && timer > 0)
        {
            CalculateSpread();

            lastCheck = Time.time;            
        }

        Vector3 direction = target - poolObject.transform.position;
        poolObject.transform.rotation = Quaternion.RotateTowards(poolObject.transform.rotation, 
            Quaternion.LookRotation(direction + spreadVector), Time.time * 0.01f);
 
        poolObject.transform.position += speed * Time.fixedDeltaTime * poolObject.transform.forward;
        timer -= Time.fixedDeltaTime;
    }

    public void FollowTarget()
    {
        if (Time.time > lastCheck + delay)
        {
            CalculateSpread();
            lastCheck = Time.time;            
        }

        if (timer > 0)
        {
            poolObject.transform.rotation = Quaternion.RotateTowards(poolObject.transform.rotation,
                Quaternion.LookRotation(spreadVector), Time.time * 0.01f);
        }
        else 
        {
            Vector3 direction = homingTarget.transform.position - poolObject.transform.position;
            poolObject.transform.rotation = Quaternion.RotateTowards(poolObject.transform.rotation,
                Quaternion.LookRotation(direction), Time.time * 1f);
        }

        poolObject.transform.position += speed * Time.fixedDeltaTime * poolObject.transform.forward;
        timer -= Time.fixedDeltaTime;
    }

    public void CalculateSpread()
    {
        spreadVector = new(
            Random.Range(-spread, spread),
            Random.Range(-spread / 4, spread / 4),
            Random.Range(-spread, spread));
    }
}
