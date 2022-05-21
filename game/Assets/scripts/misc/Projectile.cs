using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Vector3 target;
    public float speed = 10f;
    private Rigidbody bullet;
    private Collider bulletCollider;

    // Start is called before the first frame update
    void Start()
    {
        bullet = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != Vector3.zero)
        {
            bullet.velocity = transform.TransformDirection(Vector3.forward) * speed;
            
            //Set bullet lifetime
            this.Wait(3 , () => {
                transform.localScale = Vector3.zero;
                bulletCollider.enabled = false;
            });
        }
        else 
        {
            bulletCollider.enabled = true;
            bullet.velocity = Vector3.zero;
        }
    }

    //Bullet collision
    private void OnTriggerEnter(Collider other)
    {
        transform.localScale = Vector3.zero;
    }
}
