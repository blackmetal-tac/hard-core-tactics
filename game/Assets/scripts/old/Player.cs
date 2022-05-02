using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
    //2d position
	public Vector2 gridPosition = Vector2.zero;
	
	public Vector3 moveDestination;
		
    //movement range
	public int movementPerActionPoint = 5;

    //range
	public int attackRange = 1;
	
	public bool moving = false;
	public bool attacking = false;	
	
    //stats
	public string playerName = "George";
	public int HP = 25;
	
	public float attackChance = 0.75f;
	public float defenseReduction = 0.15f;
	public int damageBase = 5;
	public float damageRollSides = 6; //d6
	
	public int actionPoints = 2;
	
	//
	public List<Vector3> positionQueue = new List<Vector3>();	
	
	void Start () {
		moveDestination = transform.position;
	}
	
    //death
	public void Update () {
        if (HP <= 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
           // transform.GetComponent<Renderer>().material.color = Color.red;
        }
    }
	
	public virtual void TurnUpdate () {
		if (actionPoints <= 0) {
			actionPoints = 2;
			moving = false;
			attacking = false;			
			GameManager.instance.nextTurn();
		}
	}
	
	public virtual void TurnOnGUI () {
		
	}

    public void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontStyle = FontStyle.Bold;
        myStyle.normal.textColor = Color.green;  
        
        //show hp
        Vector3 location = Camera.main.WorldToScreenPoint(transform.position) + Vector3.up * 50;            
        GUI.Label(new Rect(location.x, Screen.height - location.y, 40 ,30), HP.ToString(), myStyle);       
    }
}
