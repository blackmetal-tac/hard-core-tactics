using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float Damage;
    private Rigidbody _projectileBody;
    private PoolObject _poolObject;
    private Collider _projectileCollider;

    // Start is called before the first frame update
    void Start()
    {
        _poolObject = GetComponentInParent<PoolObject>();
        _projectileBody = GetComponent<Rigidbody>();
        _projectileCollider = GetComponent<Collider>();
    }

    void FixedUpdate()
    {
        if (_projectileBody.velocity != Vector3.zero)
        {
            _projectileCollider.enabled = true;
        }        
    }

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(Damage);
        }
        else if (collider.gameObject.layer == 17)
        {
            collider.GetComponent<Shield>().TakeDamage(Damage);
        }

        if (collider.gameObject.layer != 17)
        {
            //_projectileCollider.enabled = false;
            //Debug.Log("disabled");
        }

        Debug.Log(collider.name);
        _projectileBody.velocity = Vector3.zero;
        _poolObject.ReturnToPool();
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == 17)
        {
            _projectileCollider.enabled = false;
            Debug.Log("enabled");
        }
    }
}
