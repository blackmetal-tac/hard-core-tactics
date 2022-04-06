using UnityEngine;
using System.Collections;

public class UserPlayer : Player 
{
    //move
    readonly float moveSpeed = 2.5f;

    //targeting
    TargetController targetCont;
    GameObject crosshair;

    Animator anim;

    private void Start()
    {
        crosshair = GameObject.Find("Crosshair");
        targetCont = crosshair.GetComponent<TargetController>();

        anim = GetComponent<Animator>();
    }    

    void LateUpdate () 
    {
        /* if (GameManager.instance.players[GameManager.instance.currentPlayerIndex] == this)
         {
             transform.GetComponent<Renderer>().material.color = Color.green;

         } else {
             transform.GetComponent<Renderer>().material.color = Color.white;
         }*/
        if (!targetCont.targeting)
        {
            //enable animator 
            anim.enabled = true;
        }

        base.Update();
	}
	
    //next turn
	public override void TurnUpdate ()
	{
		//highlight				
		if (positionQueue.Count > 0) 
        {
			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) 
            {
				transform.position = positionQueue[0];
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) 
                {
					actionPoints--;
				}
			}
			
		}		
		base.TurnUpdate ();
	}

    IEnumerator Shoot()
    {       
        while (GameManager.instance.attack == false)
        {
            GameManager.instance.attack = true;
            yield return new WaitForSeconds(1);            
            GameManager.instance.nextTurn();
            yield break;
        }       
    }

    public override void TurnOnGUI () 
    {
		float buttonHeight = 30;
		float buttonWidth = 100;

        //attack button
        Rect buttonRect = new Rect(0, Screen.height - buttonHeight * 2, buttonWidth, buttonHeight);
       
        if (GUI.Button(buttonRect, "Attack"))
        {  
            if (targetCont.target)
            {
               // GameManager.instance.attack = true;
                StartCoroutine(Shoot());                
                //GameManager.instance.Attack();
                //GameManager.instance.nextTurn();
            }
           /* else 
            {             
                GameManager.instance.Attack();
                GameManager.instance.nextTurn();

                //targeting off
                targetCont.targeting = false;
            }*/
            
        }

        //move button
        buttonRect = new Rect(0, Screen.height - buttonHeight * 4, buttonWidth, buttonHeight);
        
        if (GUI.Button(buttonRect, "Move")) 
        {
			if (!moving) 
            {
				GameManager.instance.removeTileHighlights();
				moving = true;
				attacking = false;                   
                GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint, false);
			} else
            {
				moving = false;
				attacking = false;
				GameManager.instance.removeTileHighlights();
			}
		}
        
		//targeting button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 3, buttonWidth, buttonHeight);
		
		if (GUI.Button(buttonRect, "Targeting")) 
        {
            //initialize targeting
            targetCont.targeting = !targetCont.targeting;            
        }

        //end turn button
        buttonRect = new Rect(0, Screen.height - buttonHeight * 1, buttonWidth, buttonHeight);		
		
		if (GUI.Button(buttonRect, "End Turn")) 
        {
			GameManager.instance.removeTileHighlights();
			actionPoints = 2;           

            moving = false;
			attacking = false;
            GameManager.instance.nextTurn();
		}
		
		base.TurnOnGUI ();
	}
}
