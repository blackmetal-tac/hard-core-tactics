using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 10f; 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale != Vector3.zero) 
        {
            transform.position += transform.forward * (Time.deltaTime / speed);
        }      

        this.Wait(3, () => {
            transform.localScale = Vector3.zero;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.localScale = Vector3.zero;
    }
}
