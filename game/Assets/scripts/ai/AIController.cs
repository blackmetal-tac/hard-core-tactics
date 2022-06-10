using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameObject firePoint, projectile;    
    public int burstSize;
    public float fireDelay;
    public float fireRate;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.inAction)
        {
            this.FireBurst(projectile, firePoint, fireDelay, burstSize, fireRate);
        }
    }
}
