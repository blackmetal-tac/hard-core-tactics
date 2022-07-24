using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    private GameManager gameManager;
    private ShrinkBar shrinkBar;
    private NavMeshAgent navMeshAgent;

    // Stats    
    public float HP, shield, shieldRegen, heat, cooling, heatCheck = 1f;
    public int safeHeat = 5, walkDistance;

    private float rotSpeed;

    private Vector3 direction; // Rotate body to the enemy

    public List<WPNManager> weaponList;
    private WeaponUI weaponUI;

    public float moveSpeed { set; get; }
    public float spread { set; get; }
    public float shrinkTimer { set; get; }
    public bool isDead; // Death trigger

    private float lastCheck = 0f;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        rotSpeed = 0.5f;
        moveSpeed = 0.1f;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();

        weaponList.Add(transform.Find("Torso").Find("RightArm").Find("RightArmWPN").GetComponentInChildren<WPNManager>());
        weaponList[0].unitManager = this; 

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
            }
            else
            {
                HP = 1;
            }

            if (heat > 0)
            { 
                heat -= Time.deltaTime * 0.6f;
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
            shrinkBar.UpdateShield();
        }

        // Heat dissipation
        if ((heat > 0) && gameManager.inAction && !isDead)
        {
            heat -= Time.deltaTime * cooling;
            shrinkBar.UpdateHeat();

            if (heat > 0.7f && Time.time > lastCheck + heatCheck) // Roll heat penalty
            {
                OverheatRoll();
                lastCheck = Time.time;
            }
            else if (heat >= 1f)
            {
                Overheat();
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

        if (weaponList[0].downTimer <= 0)
        {
            weaponList[0].FireBurst(target);
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

    private void OverheatRoll()
    {
        int rollHeat = Random.Range(1,10);
        if (safeHeat < rollHeat)
        {
            Overheat();
        }
    }

    private void Overheat()
    {
        int wpnIndex = Random.Range(0, 5);
        weaponList[wpnIndex].downTimer = 2;

        if (transform.parent.name == "Player")
        {
            weaponUI.weaponMasks[wpnIndex].transform.localScale = Vector3.one;
        }
    }
}
