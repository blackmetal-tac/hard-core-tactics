using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    private WPNManager wpnManager;
    private Rigidbody bullet;
    private PoolObject poolObject;

    // Start is called before the first frame update
    void Start()
    {
        wpnManager = GetComponentInParent<WPNManager>();
        poolObject = GetComponent<PoolObject>();
        bullet = GetComponent<Rigidbody>();
    } 

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body") 
        {
            collider.GetComponent<UnitManager>().TakeDamage(wpnManager.damage);
        }

        bullet.velocity = Vector3.zero;       
        poolObject.ReturnToPool();

        //Debug.Log(collider.name);
    }
}
