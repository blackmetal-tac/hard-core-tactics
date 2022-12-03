using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float Damage;
    [HideInInspector] public string BulletID;
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
        else if (collider.gameObject.layer == 17 && BulletID != collider.GetComponent<Shield>().ShieldID)
        {
            Debug.Log(collider.transform.parent.transform.parent.name);
            collider.GetComponent<Shield>().TakeDamage(Damage);
        }

        _projectileBody.velocity = Vector3.zero;
        _poolObject.ReturnToPool();
    }
}
