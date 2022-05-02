using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    //arms rotation
    float rotSpeed = 1000.0f;

    Vector3 targetPos;
    GameObject aimTarget;

    //animation
    Animator anim;

    //player rotation script
    TargetRotation tarRot;

    private void LateUpdate()
    {
        if (tarRot.rotationEnd)
        {
            //freeze y target
            targetPos = new Vector3(tarRot.target.x, transform.position.y, tarRot.target.z);

            if (tarRot.aim)
            {
                //disable animator for arms aim
                anim.enabled = false;
            }                  

            Quaternion lookAT = Quaternion.LookRotation(targetPos - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAT, rotSpeed * Time.deltaTime);
        }
        else if (!tarRot.aim)
        {
            //disable animator for arms aim
            anim.enabled = true;
        }

    }

    void Update()
    {
        anim = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<Animator>();
        tarRot = GameManager.instance.players[GameManager.instance.currentPlayerIndex].GetComponent<TargetRotation>();        
    }    
}
