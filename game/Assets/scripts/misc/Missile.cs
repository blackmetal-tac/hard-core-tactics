using UnityEngine;
using OWS.ObjectPooling;

public class Missile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public Vector3 target;
    [HideInInspector] public Rigidbody missileBody;
    [HideInInspector] public float timer = 2f;
    private PoolObject poolObject;
    private readonly float spread = 0.5f, delay = 1f;
    private float lastCheck;
    private Vector3 spreadVector;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponentInParent<PoolObject>();
        missileBody = GetComponent<Rigidbody>();

        spreadVector = new(
            Random.Range(-spread, spread) + target.x - poolObject.transform.position.x,
            Random.Range(-spread / 4, spread / 4) + target.y - poolObject.transform.position.y,
            Random.Range(-spread, spread) + target.z - poolObject.transform.position.z);
    }

    void Update()
    {
        if (poolObject.transform.localScale != Vector3.zero)
        {
            MoveTowards();          
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
            timer = 2f;
            missileBody.velocity = Vector3.zero;
            poolObject.ReturnToPool();
        }
    }

    public void MoveTowards()
    {        
        if (Time.time > lastCheck + delay && timer > 0)
        {
            spreadVector = new(
                Random.Range(-spread, spread),
                Random.Range(-spread / 4, spread / 4),
                Random.Range(-spread, spread));

            lastCheck = Time.time;
            timer -= Time.deltaTime;
        }

        Vector3 direction = target - poolObject.transform.position;
        poolObject.transform.rotation = Quaternion.RotateTowards(
            poolObject.transform.rotation, Quaternion.LookRotation(direction + spreadVector), Time.time * 0.01f);
 
        poolObject.transform.position += speed * Time.deltaTime * poolObject.transform.forward;
    }
}
