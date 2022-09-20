using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    private Rigidbody projectileBody;
    private PoolObject poolObject;
    private Collider projectileCollider;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponentInParent<PoolObject>();
        projectileBody = GetComponent<Rigidbody>();
        projectileCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        if (projectileBody.velocity != Vector3.zero)
        {
            projectileCollider.enabled = true;
        }        
    }

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);
        }
        else if (collider.gameObject.layer == 17)
        {
            collider.GetComponent<Shield>().TakeDamage(damage);
        }

        if (collider.gameObject.layer != 17)
        {
            //projectileCollider.enabled = false;
            //Debug.Log("disabled");
        }

        Debug.Log(collider.name);
        projectileBody.velocity = Vector3.zero;
        poolObject.ReturnToPool();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == 17)
        {
            projectileCollider.enabled = false;
            Debug.Log("enabled");
        }
    }
}
