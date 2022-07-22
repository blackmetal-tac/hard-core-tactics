using UnityEngine;
using UnityEngine.AI;

public class UnitManager : MonoBehaviour
{
    private GameManager gameManager;
    private ShrinkBar shrinkBar;
    private NavMeshAgent navMeshAgent;

    // Stats    
    public float HP, shield, shieldRegen, heat, cooling;
    public int safeHeat = 5, walkDistance;

    private float rotSpeed, delay = 3f;

    private Vector3 direction; // Rotate body to the enemy
    private WPNManager rightWPN;

    public float moveSpeed { set; get; }
    public float spread { set; get; }
    public float shrinkTimer { set; get; }
    public bool isDead; // Death trigger

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        rotSpeed = 0.5f;
        moveSpeed = 0.1f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        rightWPN = transform.Find("Torso").Find("RightArm").Find("RightArmWPN").GetComponentInChildren<WPNManager>();
        rightWPN.unitManager = this; 

        navMeshAgent = transform.GetComponentInParent<NavMeshAgent>();
        shrinkBar = GetComponentInChildren<ShrinkBar>();

        // Load HP, Shield, Heat bars
        this.Progress(gameManager.loadTime, () => {
            if (shield < 1)
            { 
                shield += Time.deltaTime * 0.6f; 
            }
            else
            {
                shield = 1;
            }

            if (HP < 1)
            {
                HP += Time.deltaTime * 0.6f;
                HP = Mathf.Round(100 * HP) / 100;               
            }
            else
            {
                HP = 1;
            }

            if (heat > 0)
            { 
                heat -= Time.deltaTime * 0.6f;
                heat = Mathf.Round(100 * heat) / 100;
            }
            else
            {
                heat = 0;
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
            float heatCheck = 0f;
            if (Time.time > heatCheck + 10)
            {
                heat -= 0.05f;
                //heat = Mathf.Round(100 * heat) / 100;
                shrinkBar.UpdateHeat();
                heatCheck = Time.time;
            }

            if (heat > 0.7f) // Roll heat penalty
            {
                float lastCheck = 0f;
                if (Time.time > lastCheck + delay)
                {
                    Overheat();
                    lastCheck = Time.time;
                }
            }
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
                moveSpeed = Mathf.Round(100 * moveSpeed) / 100;
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

    public void StartShoot(GameObject target)
    {
        // Rotate body to target
        direction = target.transform.position - transform.position;
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, Quaternion.LookRotation(direction), Time.time * rotSpeed);

        if (!rightWPN.isDown)
        {
            rightWPN.FireBurst(target);
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

    private void Overheat()
    {
        int rollHeat = Random.Range(0,10);        

        if (safeHeat < rollHeat)
        {            
            int wpnIndex = Random.Range(1, 6);
            Debug.Log(wpnIndex);
            rightWPN.isDown = true;

            if (wpnIndex == 1)
            {
               
            }
        }
    }
}
