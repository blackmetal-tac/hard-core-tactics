using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetRotation : MonoBehaviour
{ 
    //targeting
    public bool targeting = false;

    //start arms targeting
    public bool rotationEnd = false;
    public bool aim = false;

    //target
    public Vector3 target;
    Vector3 targetPos;

    //speet of rotation
    readonly float rotSpeed = 80.0f;    

    //animation
    Animator anim;

    private void Start()
    {       
        anim = GetComponent<Animator>();
    }   

    public void LateUpdate()
    {
        if (targeting && GameManager.instance.players[GameManager.instance.currentPlayerIndex].HP > 0)
        {
            Rotate(target);
        }
       
        if (!targeting)
        {
            rotationEnd = false;
            anim.SetBool("Targeting", false);
        }
    }    

    //rotation
    public void Rotate(Vector3 target)
    {
        targetPos = new Vector3(target.x, transform.position.y, target.z);
        Quaternion lookAT = Quaternion.LookRotation(targetPos - transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAT, rotSpeed * Time.deltaTime);

        //rotation anim on
        anim.SetBool("Rotation", true);
        anim.SetBool("Targeting", false);
        anim.SetBool("TargetingTurn", false);

        if (lookAT == transform.rotation)
        {
            //rotation anim off
            anim.SetBool("Rotation", false);
            anim.SetBool("Targeting", true);
            anim.SetBool("TargetingTurn", false);

            StartCoroutine(Arms());
        }
        else 
        {
            //arms off
            rotationEnd = false;
        }

    }

    IEnumerator Arms()
    {
        if (aim)
        {
            yield return new WaitForSeconds(0.4f);

            //arms on
            rotationEnd = true;

            yield break;
        }
        
    }

}
