using UnityEngine;

public class AIController : MonoBehaviour
{
    ObjectPooler objectPooler;
    public GameObject firePoint;    
    public int burstSize;
    public float fireDelay;
    public float fireRate;

    // Start is called before the first frame update
    void Start()
    {
        //DONT'T FORGET TO INSTANSIATE when shooting        
        objectPooler = ObjectPooler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.inAction)
        {
            this.FireBurst(objectPooler, "Bullet", firePoint, fireDelay, burstSize, fireRate);
        }
    }
}
