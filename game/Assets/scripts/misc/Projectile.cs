using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public float damage;
    private Rigidbody projectileBody;
    private PoolObject poolObject;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponentInParent<PoolObject>();
        projectileBody = GetComponent<Rigidbody>();
    } 

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body")
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);
        }

        projectileBody.velocity = Vector3.zero;
        poolObject.ReturnToPool();
    }
}
