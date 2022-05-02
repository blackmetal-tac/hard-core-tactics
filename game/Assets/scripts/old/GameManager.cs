using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{    
    //instantiate
    public static GameManager instance;
    
    //move camera script
    CameraFocus camFocus;

    //player rotation 
    TargetRotation tarRot;

    //targeting
    TargetController targetCont;
    GameObject crosshair;

    public GameObject TilePrefab;
	public GameObject UserPlayerPrefab;
	public GameObject AIPlayerPrefab;

    public List<List<Tile>> map = new List<List<Tile>>();
    public List<Player> players = new List<Player>();
    public List<AIPlayer> enemies = new List<AIPlayer>();

    //map
    public int mapSize = 11;	

    //current player
    public int currentPlayerIndex = 0;    
    bool newPlayer = false;

    //attack target
    public AIPlayer target;

    //weapon attack
    public bool attack = false;

    //get weapon 
    Transform weapon;
    public Gun gun;

    void Start ()
    {
        //instance
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        //generation
        generateMap();
		generatePlayers();

        //get scripts
        camFocus = Camera.main.GetComponent<CameraFocus>();
        tarRot = players[currentPlayerIndex].GetComponent<TargetRotation>();
        crosshair = GameObject.Find("Crosshair");
        targetCont = crosshair.GetComponent<TargetController>();
    }
    
    void Update ()
    {
        //remove dead enemy
        foreach (AIPlayer e in enemies)
        {
            if (e.HP <= 0)
            {
                enemies.Remove(e);
            }
        }

        //check alive player
        if (players[currentPlayerIndex].HP > 0)
        {
            players[currentPlayerIndex].TurnUpdate();           
        } 
        else
        {
            nextTurn();
            newPlayer = true;
        }

        //get current weapon
        //weapon = GameManager.instance.players[GameManager.instance.currentPlayerIndex].transform.Find("Machine");
        gun = gameObject.GetComponent<Gun>();

    }
	
    //enable gui
	void OnGUI ()
    {
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnOnGUI();
	}

    //pass turn
    public void nextTurn()
    {
        if (currentPlayerIndex + 1 < players.Count)
        {
            //change player
            currentPlayerIndex++;
        }
        else
        {
            currentPlayerIndex = 0;
        }

        camFocus.stop = false;

        //targeting off
        targetCont.targeting = false;
        attack = false;
    }
	
    //tile highlighting
	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance, bool ignorePlayers = true) {
        List<Tile> highlightedTiles = new List<Tile>();
        if (ignorePlayers) highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
        else highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance, 
            players.Where(x => x.gridPosition != originLocation).Select(x => x.gridPosition).ToArray());
		
		foreach (Tile t in highlightedTiles) {
			t.transform.GetComponent<Renderer>().material.color = highlightColor;
		}
	}
	
    //highlite remover
	public void removeTileHighlights() {
		for (int i = 0; i < mapSize; i++) {
			for (int j = 0; j < mapSize; j++) {
				if (!map[i][j].impassible) map[i][j].transform.GetComponent<Renderer>().material.color = Color.white;
			}
		}
	}

    //movement
    public void moveCurrentPlayer(Tile destTile)
    {      
        if (destTile.transform.GetComponent<Renderer>().material.color != Color.white && !destTile.impassible)
        {
            removeTileHighlights();
            players[currentPlayerIndex].moving = false;

            foreach (Tile t in TilePathFinder.FindPath(map[(int)players[currentPlayerIndex].gridPosition.x][(int)players[currentPlayerIndex].gridPosition.y],
                destTile, players.Where(x => x.gridPosition != players[currentPlayerIndex].gridPosition).Select(x => x.gridPosition).ToArray()))
            {                   
                //path
                players[currentPlayerIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position + 0.5f * Vector3.up);
                 
            }                    
            players[currentPlayerIndex].gridPosition = destTile.gridPosition;            
        }        
    }

    //ranged attack
    public void Attack()
    {   
        //find target
        foreach (AIPlayer p in enemies)
        {
            if (p.transform.position == tarRot.target)
            {
                target = p;
            }
        }        

        //attack chance
        bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance;

        if (hit)
        {
            //damage logic
            int amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));

            //damage
            target.HP -= amountOfDamage;
            Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!");
        }
        else
        {
            //miss
            Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
        }
        
    }

    //old attack
    public void attackWithCurrentPlayer(Tile destTile) {
		if (destTile.transform.GetComponent<Renderer>().material.color != Color.white && !destTile.impassible) {
			
			Player target = null;
			foreach (Player p in players) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;                    
                }
			}
			
			if (target != null) {               

                if (players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - 1 && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + 1 &&
					players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - 1 && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + 1) {
					players[currentPlayerIndex].actionPoints--;
					
					removeTileHighlights();
					players[currentPlayerIndex].attacking = false;                    

                    //attack logic
                    //roll to hit
                    bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance;

                    if (hit)
                    {
                        //damage logic
                        int amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));

                        target.HP -= amountOfDamage;
                        
                        Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!");
                    }
                    else
                    {
                        Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
                    }


                } else {
					Debug.Log ("Target is not adjacent!");
				}
			}
		} else {
			Debug.Log ("destination invalid");
		    }
	}
	
    //map generation
	void generateMap() {
		map = new List<List<Tile>>();
		for (int i = 0; i < mapSize; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
                Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3(i - Mathf.Floor(mapSize / 2), 0, -j + Mathf.Floor(mapSize / 2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
                tile.gridPosition = new Vector2(i, j);
				row.Add (tile);
			}
			map.Add(row);
		}
	}
	
    //add players / enemies
	void generatePlayers() {
		UserPlayer player;
		
		player = ((GameObject)Instantiate(UserPlayerPrefab, new Vector3(0 - Mathf.Floor(mapSize/2),0.5f, -0 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3(0, 180, 0)))).GetComponent<UserPlayer>();
        player.gridPosition = new Vector2(0,0);
        player.playerName = "Player 1";
        player.damageBase = 10;

        players.Add(player);
         
        player = ((GameObject)Instantiate(UserPlayerPrefab, new Vector3((mapSize-1) - Mathf.Floor(mapSize/2),0.5f, -(mapSize-1) + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
        player.gridPosition = new Vector2(mapSize-1,mapSize-1);
        player.playerName = "Player 2";

        players.Add(player);

        AIPlayer aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(6 - Mathf.Floor(mapSize/2),0.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
        aiplayer.gridPosition = new Vector2(6, 4);
        aiplayer.playerName = "Bot 1";

        players.Add(aiplayer);
        enemies.Add(aiplayer);

        aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(4 - Mathf.Floor(mapSize / 2), 0.5f, -4 + Mathf.Floor(mapSize / 2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
        aiplayer.gridPosition = new Vector2(4, 4);
        aiplayer.playerName = "Bot 2";

        players.Add(aiplayer);
        enemies.Add(aiplayer);
    }

}


