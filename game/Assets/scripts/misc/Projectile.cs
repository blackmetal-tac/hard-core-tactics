using UnityEngine;
using OWS.ObjectPooling;

public class Projectile : MonoBehaviour
{
    public float damage;
    public float heat;
    public float speed;
    public float size;

    private Rigidbody bullet;
    private PoolObject poolObject;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponent<PoolObject>();
        bullet = GetComponent<Rigidbody>();
    } 

    // Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Body") 
        {
            collider.GetComponent<UnitManager>().TakeDamage(damage);
        }

        bullet.velocity = Vector3.zero;       
        poolObject.ReturnToPool();

        //Debug.Log(collider.name);
    }
}
