using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    UserPlayer player;
    GameObject barrel;
    
    ParticleSystem flash;

    public int damage = 1;
    public GameObject bullet;

    //fire rate
    private float timer = 0.0f;
    private float fireRate = 0.3f;  

    private void Update()
    {
        barrel = GameManager.instance.players[GameManager.instance.currentPlayerIndex].transform.GetChild(0).transform.GetChild(0).transform.GetChild(2).transform.GetChild(2).transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).transform.GetChild(3).gameObject;
        Debug.DrawLine(barrel.transform.position, barrel.transform.forward * 1000, Color.red);
        flash = barrel.GetComponentInChildren<ParticleSystem>();

        if (GameManager.instance.attack && Time.time > timer)
        {
            timer = Time.time + fireRate;
            //Shoot();

            flash.Play();

            //spawn bullet
            Instantiate(bullet, barrel.transform.position, barrel.transform.rotation);           
        }        

    }     

}
