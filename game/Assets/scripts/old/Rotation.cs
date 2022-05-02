using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    Vector3 lastPos;
    Vector3 targetPos;
    Vector3 offset;

    readonly float threshold = 0.0f;

    //speed
    readonly float rotSpeed = 250.0f;

    //distance to look forward
    readonly float horizon = 1000.0f;

    //animation
    Animator anim;

    void Start()
    {           
        lastPos = transform.position;
        anim = GetComponent<Animator>();
    } 

    void LateUpdate()
    {
        LookForward();                    
    }  

    //player rotation
    void LookForward()
    {
        offset = transform.position - lastPos;
        if (offset.x > threshold)
        {
            //move anim
            anim.SetBool("IsWalking", true);

            targetPos = new Vector3(transform.position.x + horizon, transform.position.y, transform.position.z);
            Quaternion lookAT = Quaternion.LookRotation(targetPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAT, rotSpeed * Time.deltaTime);
            lastPos = transform.position;
            offset = transform.position - lastPos;
        }
        else if (offset.x < threshold)
        {
            //move anim
            anim.SetBool("IsWalking", true);

            targetPos = new Vector3(transform.position.x - horizon, transform.position.y, transform.position.z);
            Quaternion lookAT = Quaternion.LookRotation(targetPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAT, rotSpeed * Time.deltaTime);
            lastPos = transform.position;
            offset = transform.position - lastPos;
        }
        else if (offset.z > threshold)
        {
            //move anim
            anim.SetBool("IsWalking", true);

            targetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + horizon);
            Quaternion lookAT = Quaternion.LookRotation(targetPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAT, rotSpeed * Time.deltaTime);
            lastPos = transform.position;
            offset = transform.position - lastPos;
        }
        else if (offset.z < threshold)
        {
            //move anim
            anim.SetBool("IsWalking", true);

            targetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - horizon);
            Quaternion lookAT = Quaternion.LookRotation(targetPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAT, rotSpeed * Time.deltaTime);
            lastPos = transform.position;
            offset = transform.position - lastPos;
        }
        else
        {
            //move anim
            anim.SetBool("IsWalking", false);
        }

    }

}
