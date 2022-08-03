using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour
{
    private GameManager gameManager;
    private ShrinkBar shrinkBar;
    private NavMeshAgent navMeshAgent;

    // Stats    
    public float HP, shield, shieldRegen, heat, cooling;
    [Range(0, 1)] public float heatTreshold, heatCheckTime;
    [Range(0, 10)] public int heatSafeRoll;
    public int walkDistance;

    private Vector3 direction; // Rotate body to the enemy
    private readonly float rotSpeed = 0.5f;

    public List<WPNManager> weaponList;
    public int weaponCount = 0;
    private WeaponUI weaponUI;

    [HideInInspector] public float moveSpeed = 0.1f;
    [HideInInspector] public float spread;
    [HideInInspector] public float shrinkTimer;
    public bool isDead = false; // Death trigger
    private float lastCheck;

    // Start is called before the first frame update
    void Start()
    { 
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        weaponUI = GameObject.Find("WeaponUI").GetComponent<WeaponUI>();
        navMeshAgent = transform.GetComponentInParent<NavMeshAgent>();
        shrinkBar = GetComponentInChildren<ShrinkBar>();

        /* Fill the list of all weapons on this unit (ORDER: rigth arm, left arm, rigth top,
              left top, rigth shoulder, left shoulder) */
        weaponList.Add(transform.Find("Torso").Find("RightArm").Find("RightArmWPN").GetComponentInChildren<WPNManager>());
        weaponList.Add(transform.Find("Torso").Find("LeftArm").Find("LeftArmWPN").GetComponentInChildren<WPNManager>());
        weaponList.Add(transform.Find("Torso").Find("RightShoulderTopWPN").GetComponentInChildren<WPNManager>());
        weaponList.Add(transform.Find("Torso").Find("LeftShoulderWPN").GetComponentInChildren<WPNManager>());
        weaponList.Add(transform.Find("Torso").Find("RightArm").Find("RightShoulderWPN").GetComponentInChildren<WPNManager>());
        weaponList.Add(transform.Find("Torso").Find("LeftArm").Find("LeftShoulderWPN").GetComponentInChildren<WPNManager>());

        // ??? assign unit manager for each weapon
        foreach (WPNManager weapon in weaponList)
        {
            if (weapon != null)
            {
                weapon.unitManager = this;
                weaponCount += 1;
            }            
        }

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

        foreach (WPNManager weapon in weaponList)
        {
            if (weapon != null)
            {
                weapon.FireBurst(target);
            }            
        }
    }

    // Do actions in Update
    public void StartAction()
    {
        // Shield regeneration
        if (shield < 1 && !isDead)
        {
            shield += Time.deltaTime * shieldRegen;
            shrinkBar.UpdateShield();
        }

        // Heat dissipation
        if (heat > 0 && !isDead)
        {
            heat -= Time.deltaTime * cooling;
            shrinkBar.UpdateHeat();

            if (heat > heatTreshold && Time.time > lastCheck + heatCheckTime) // Roll heat penalty
            {
                OverheatRoll();
                lastCheck = Time.time;
            }
            else if (heat >= 1f && Time.time > lastCheck + 0.3f)
            {
                Overheat();
                lastCheck = Time.time;
            }
        }

        if (!isDead)
        {
            shrinkBar.UpdateHealth();
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
            GetComponent<Collider>().enabled = false;
            transform.localScale = Vector3.zero;
        }
    }

    // Roll chance for overheat
    private void OverheatRoll()
    {
        int rollHeat = Random.Range(1,10);
        if (heatSafeRoll < rollHeat)
        {
            Overheat();
        }
    }

    // Roll a weapon to overheat
    private void Overheat()
    {
        int wpnIndex = Random.Range(0, weaponCount);
        if (weaponList[wpnIndex] != null && weaponList[wpnIndex].downTimer <= 0)
        {
            weaponList[wpnIndex].downTimer = 3;
            weaponList[wpnIndex].burstSize = weaponList[wpnIndex].weaponModes[0].fireMode;

            if (transform.parent.name == "Player")
            {
                weaponUI.WeaponDown(wpnIndex, weaponList[wpnIndex].downTimer);
            }
        }
    }

    // Update timers for overheated weapon
    public void UpdateTimer()
    {
        for (int i = 0; i < weaponList.Count; i++)
        {
            if (weaponList[i] != null)
            {
                weaponList[i].downTimer -= 1;

                if (transform.parent.name == "Player" && weaponList[i].downTimer > 0)
                {
                    weaponUI.UpdateStatus(i, weaponList[i].downTimer); // ???
                }
                else if (transform.parent.name == "Player" && weaponList[i].downTimer <= 0)
                {
                    weaponUI.WeaponUp(i);
                }
            }
        }
    }
}
