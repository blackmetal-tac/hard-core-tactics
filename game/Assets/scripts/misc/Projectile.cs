using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    public float damage = 0.03f;
    public float speed = 10f;

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
            bullet.velocity = transform.TransformDirection(Vector3.forward) * speed;
            
            // Set bullet lifetime
            this.Wait(3 , () => {
                transform.localScale = Vector3.zero;
                bulletCollider.enabled = false;
                poolObject.ReturnToPool();            
            });
        }
        else 
        {
            bulletCollider.enabled = false;
            bullet.velocity = Vector3.zero;
        }

        if (bullet.velocity != Vector3.zero) 
        {
            bulletCollider.enabled = true;
        }
    }

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Enemy" || collider.name == "Body") 
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);

            // Reset HP bar damage animation
            collider.GetComponent<UnitManager>().shrinkTimer = 0.5f;                        
        }

        bulletCollider.enabled = false;
        transform.localScale = Vector3.zero;
        poolObject.ReturnToPool();
    }
}
