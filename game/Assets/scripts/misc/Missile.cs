using UnityEngine;
using OWS.ObjectPooling;

public class Missile : MonoBehaviour
{
    public float damage { get; set; }
    public float speed { get; set; }
    public Vector3 target { get; set; }
    private PoolObject poolObject;
    private float spread;

    // Start is called before the first frame update
    void Start()
    {
        poolObject = GetComponent<PoolObject>();
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

        poolObject.ReturnToPool();
    }

    public void MoveTowards()
    {
        Vector3 spreadVector = new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread / 2, spread / 2),
            Random.Range(- spread, spread));

        transform.position = Vector3.MoveTowards(transform.position, target + spreadVector, (speed / 10) * Time.deltaTime);
    }
}
