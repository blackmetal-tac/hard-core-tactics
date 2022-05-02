using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFocus : MonoBehaviour
{
    //focus target
    Vector3 targetPos;
    Vector3 moveVelocity;

    //speed
    readonly float damp = 1.0f;
    
    //rotation switch
    public bool stop = false;

    //angle
    Quaternion reset;       

    private void Start()
    {
        reset = transform.rotation; 
    }

    public void LateUpdate()
    {
        if (transform.position == targetPos - new Vector3(0, 0, 2))
        {
            stop = true;
        }
        else if (Input.anyKeyDown)
        {
            stop = true;
        }      
        
        Move(stop);               
    }

    //move to current player
    void Move(bool stop)
    {
        if (!stop)
        {
            GameObject target = GameManager.instance.players[GameManager.instance.currentPlayerIndex].gameObject;
            targetPos = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);

            transform.position = Vector3.SmoothDamp(transform.position, targetPos - new Vector3(0, 0, 2), ref moveVelocity, damp);

            //reset camera rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, reset, damp * Time.deltaTime);
        }
            
    }     

}
