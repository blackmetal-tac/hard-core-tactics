using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    // Objects
    private GameObject clickMarker;
    public GameObject projectile, firePoint;    

    // Unit stats
    public float mechSpeed = 3.5f;

    // Attack parameters
    public int burstSize;
    public float fireDelay;    
    public float fireRate;
    private UnitManager unitManager;

    // NavMesh
    public NavMeshAgent playerAgent;
    private LineRenderer walkPath;

    // Start is called before the first frame update
    void Start()
    {

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // Navmesh setup        
        playerAgent = GetComponent<NavMeshAgent>();
        walkPath = GetComponent<LineRenderer>();

        // Path
        clickMarker = GameObject.Find("ClickMarker");
        clickMarker.transform.localScale = Vector3.zero;        
        walkPath.startWidth = 0.02f;
        walkPath.endWidth = 0.02f;
        walkPath.positionCount = 0;

        unitManager = GetComponentInChildren<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!gameManager.inAction)
            {
                MoveToClick();                
            }
        }

        // If in ACTION PHASE
        if (gameManager.inAction)
        {
            unitManager.FireBurst(projectile, firePoint, fireDelay, burstSize, fireRate);
            playerAgent.speed = unitManager.moveSpeed + 0.5f;
        }

        // Draw player path
        if (playerAgent.hasPath)
        {
            DrawPath();
        }
        else 
        {
            clickMarker.transform.localScale = Vector3.zero;
        }        

        clickMarker.transform.Rotate(new Vector3(0, 0, -Time.deltaTime * 50));       
    }

    private void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool isHit = Physics.Raycast(ray, out hit);

        if(isHit)
        {
            unitManager.SetDestination(hit.point, playerAgent);
        }       
    }

    // Draw player path
    private void DrawPath()
    {
        walkPath.positionCount = playerAgent.path.corners.Length;
        walkPath.SetPosition(0, transform.position);

        if (playerAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < playerAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new Vector3(playerAgent.path.corners[i].x, playerAgent.path.corners[i].y,
                    playerAgent.path.corners[i].z);
            walkPath.SetPosition(i, pointPosition);
            clickMarker.transform.position = new Vector3(pointPosition.x, 0.05f, pointPosition.z);
            clickMarker.transform.localScale = Vector3.zero;
            clickMarker.transform.DOScale(Vector3.one * 0.2f, 0.2f);
        }
    }
}
