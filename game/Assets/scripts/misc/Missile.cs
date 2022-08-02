using UnityEngine;
using OWS.ObjectPooling;

public class Missile : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float speed;
    [HideInInspector] public Vector3 target;
    public Rigidbody missileBody;
    private PoolObject poolObject;
    private readonly float spread = 0.5f;
    private float lastBurst, fireDelay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponentInParent<PoolObject>();
        missileBody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (transform.localScale != Vector3.zero)
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

        missileBody.velocity = Vector3.zero;
        poolObject.ReturnToPool();
    }

    public void MoveTowards()
    {
        Vector3 spreadVector = new(
            Random.Range(-spread, spread),
            Random.Range(-spread / 2, spread / 2),
            Random.Range(- spread, spread));

        poolObject.transform.rotation = Quaternion.RotateTowards(
            transform.rotation, Quaternion.LookRotation(target + spreadVector), Time.time * 1f);
        poolObject.transform.position += speed * Time.deltaTime * poolObject.transform.forward;
        //missileBody.velocity = speed * poolObject.transform.forward;
    }
}
