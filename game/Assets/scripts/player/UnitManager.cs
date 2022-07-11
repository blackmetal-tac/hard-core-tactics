using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    private GameManager gameManager;
    private ShrinkBar shrinkBar;
    private NavMeshAgent navMeshAgent;

    // Stats    
    public float HP;
    public float shield;
    public float shieldRegen;
    public float heat; // Total unit heat
    public float cooling; // Cooling modifier
    public int walkDistance; // Speed (max distance)
    public float moveSpeed; // Actual agents speed
    private float rotSpeed;

    public GameObject target; // Aiming objects
    private Vector3 direction; // Rotate body to the enemy

    public WPNManager rightWPN;
    public Aiming aimingRightWPN;
    public Projectile projectileRightWPN;

    public float shrinkTimer {get; set;}

    public bool isDead; // ???

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        rotSpeed = 0.5f;
        moveSpeed = 0.1f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        rightWPN = transform.Find("Torso").transform.Find("RightArm").GetChild(0)
            .GetChild(0).GetComponent<WPNManager>();
        aimingRightWPN = rightWPN.firePoint.GetComponent<Aiming>();

        navMeshAgent = transform.GetComponentInParent<NavMeshAgent>();
        shrinkBar = GetComponentInChildren<ShrinkBar>();

        // Load HP, Shield, Heat bars
        this.Progress(gameManager.loadTime, () => {
            if (shield < 1)
            { 
                shield += Time.deltaTime * 0.6f; 
            }

            if (HP < 1)
            {
                HP += Time.deltaTime * 0.6f;
            }

            if (heat > 0)
            { 
                heat -= Time.deltaTime * 0.6f;
            }

            shrinkBar.UpdateShield();
            shrinkBar.UpdateHealth();
            shrinkBar.UpdateHeat();
        });

        // Load Heat
        this.Progress(gameManager.loadTime, () => {
            if (shield < 1)
            {
                shield += Time.deltaTime * 0.6f;
            }

            if (HP < 1)
            {
                HP += Time.deltaTime * 0.6f;
            }
        });  
    }

    private void FixedUpdate()
    {
        // Rotate body to target
        if (gameManager.inAction && !isDead)
        {            
            direction = target.transform.position - transform.position;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(direction), Time.time * rotSpeed);
        }        
    }

    // Update is called once per frame
    void Update()
    {     
        // Shield regeneration
        if ((shield < 1) && gameManager.inAction && !isDead)
        {
            shield += Time.deltaTime * shieldRegen;
            shield = Mathf.Round(100 * shield) / 100;
            shrinkBar.UpdateShield();            
        }

        // Heat dissipation
        if ((heat > 0) && gameManager.inAction && !isDead)
        {
            heat -= Time.deltaTime * cooling;
            heat = Mathf.Round(100 * heat) / 100;
            shrinkBar.UpdateHeat();
        }

        if (gameManager.inAction && !isDead)
        {
            shrinkBar.UpdateHealth();
        }
    }

    // Set move position and maximum move distance (speed)
    public void SetDestination(Vector3 target, NavMeshAgent navAgent)
    {
        NavMeshPath path = new NavMeshPath();
        navAgent.speed = 0;

        NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;
            if (segmentDistance <= walkDistance)
            {
                navAgent.SetDestination(target);
                moveSpeed = segmentDistance / gameManager.turnTime;
            }
            else
            {
                Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * walkDistance);
                NavMesh.CalculatePath(transform.position, finalPoint, NavMesh.AllAreas, path);
                navAgent.SetPath(path);
                moveSpeed = walkDistance / gameManager.turnTime;
                break;
            }
        }
    }

    // Take damage
    public void TakeDamage(float damage)
    {
        // Reset HP bar damage animation
        shrinkTimer = 0.5f;

        if (shield > 0)
        {
            shield -= damage;
        }
        else if (HP > 0)
        {
            HP -= damage;            
        }

        // Death
        if (HP <= 0)
        {
            isDead = true;
            navMeshAgent.enabled = false;
            transform.localScale = Vector3.zero;
        }
    }
}
