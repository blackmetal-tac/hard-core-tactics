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
        transform.position += transform.forward * (Time.deltaTime / speed);

        this.Wait(3, () => {
            gameObject.transform.localScale = Vector3.zero;
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        gameObject.transform.localScale = Vector3.zero;
    }
}
