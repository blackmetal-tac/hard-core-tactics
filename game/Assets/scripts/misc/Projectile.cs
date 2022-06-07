using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static Vector3 target;
    private float damage = 0.03f;
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
            bulletCollider.enabled = false;
            bullet.velocity = Vector3.zero;
        }

        if (bullet.velocity != Vector3.zero) 
        {
            bulletCollider.enabled = true;
        }
    }

    //Bullet collision
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.name == "Enemy" || collider.name == "Player") 
        {
            collider.GetComponent<CharacterStats>().TakeDamage(damage);

            //Reset HP bar damage animation
            collider.GetComponent<CharacterStats>().shrinkTimer = 0.5f;                              
        }

        bulletCollider.enabled = false;
        transform.localScale = Vector3.zero;
    }
}
