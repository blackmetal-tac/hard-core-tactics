using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float heat;
    public float speed;

    private Rigidbody bullet;
    private Collider bulletCollider;
    private PoolObject poolObject;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponent<PoolObject>();
        bullet = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != Vector3.zero)
        {
            bullet.velocity = speed * transform.TransformDirection(Vector3.forward); 
        }
    }

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body") 
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);

            // Reset HP bar damage animation
            collider.GetComponent<UnitManager>().shrinkTimer = 0.5f;                        
        }

        bulletCollider.enabled = false;
        bullet.velocity = Vector3.zero;
        transform.localScale = Vector3.zero;        
        poolObject.ReturnToPool();
    }
}
