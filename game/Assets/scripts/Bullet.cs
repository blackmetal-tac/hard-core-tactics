using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject smallExplosion;
    
    AIPlayer target;

    //bullet speed
    float speed = 20.0f;

    void Update()
    {    
        transform.position += transform.forward * (speed * Time.deltaTime);
        Destroy(gameObject, 3.0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        speed = 0.0f;        

        AIPlayer enemy = collision.gameObject.transform.GetComponent<AIPlayer>();
        if (enemy != null)
        {
            foreach (AIPlayer p in GameManager.instance.enemies)
            {
                if (p.transform.position == enemy.transform.position)
                {
                    target = p;
                }
            }

            target.HP -= GameManager.instance.gun.damage;
            Debug.Log(target.playerName + " takes " + GameManager.instance.gun.damage + " damage" + " from " + GameManager.instance.players[GameManager.instance.currentPlayerIndex].playerName);
        }

        ContactPoint contact = collision.GetContact(0);
        Instantiate(smallExplosion, contact.point, Quaternion.identity);     
        
        Destroy(gameObject);        
    }
}
