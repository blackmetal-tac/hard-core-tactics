using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetController : MonoBehaviour
{
    //Main Camera
    Camera cam; 

    //Current Focused Enemy In List
    public AIPlayer target;

    //Image Of Crosshair    
    RawImage image;

    //Keeps Track Of Lock On Status   
    bool lockedOn;

    //enables targeting
    public bool targeting = false;

    //Tracks Which Enemy In List Is Current Target
    int lockedEnemy;

    //player rotation script
    TargetRotation tarRot;

    //animation
    Animator anim;

    void Start()
    {
        cam = Camera.main;
        image = gameObject.GetComponent<RawImage>();

        lockedOn = false;
        lockedEnemy = 0;

        tarRot = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<TargetRotation>();
        anim = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<Animator>();
    }

    void LateUpdate()
    {
        //Lock On
        if (targeting && !lockedOn)
        {
            if (GameManager.instance.enemies.Count >= 1) 
            {
                lockedOn = true;
                image.enabled = true;

                //Lock On To First Enemy In List By Default
                lockedEnemy = 0;
                target = GameManager.instance.enemies[lockedEnemy];

                //targeting rotation on
                tarRot = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<TargetRotation>();
                tarRot.target = target.transform.position;
                tarRot.targeting = true;
                tarRot.aim = true;
            }
            
        }

        //Turn Off Lock On No More Enemies Are In The List
        else if ((!targeting && lockedOn) || GameManager.instance.enemies.Count == 0)
        {          
            lockedOn = false;
            image.enabled = false;
            lockedEnemy = 0;
            target = null;

            //targeting rotation off
            tarRot.targeting = false;
            tarRot.aim = false;
        }       

        //Tab To Switch Targets
        if (Input.GetKeyDown(KeyCode.Tab) && targeting)
        {
            anim.SetBool("TargetingTurn", true);            

            if (lockedEnemy == GameManager.instance.enemies.Count - 1)
            {
                //If End Of List Has Been Reached, Start Over
                lockedEnemy = 0;

                //target 
                target = GameManager.instance.enemies[lockedEnemy];

                //targeting rotation on
                tarRot = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<TargetRotation>();
                tarRot.target = target.transform.position;                
            }
            else
            {
                //Move To Next Enemy In List
                lockedEnemy++;

                //target
                target = GameManager.instance.enemies[lockedEnemy];

                //targeting rotation on
                tarRot = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<TargetRotation>();
                tarRot.target = target.transform.position;

                tarRot.aim = false;
            }
        }

        if (lockedOn)
        {
            //target = nearByEnemies[lockedEnemy];
            target = GameManager.instance.enemies[lockedEnemy];

            //Determine Crosshair Location Based On The Current Target
            gameObject.transform.position = cam.WorldToScreenPoint(target.transform.position);

            //Rotate Crosshair
            //gameObject.transform.Rotate(new Vector3(0, 0, -1));
        }
    }

    IEnumerator Arms()
    {        
        //disable animator for arms aim
        tarRot.rotationEnd = false;

        //enable animator 
        anim.enabled = true;        

        yield return new WaitForSeconds(1);

        yield break;
    }
}