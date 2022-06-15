using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private Camera camMain;
    private GameManager gameManager;

    // NavMesh
    private NavMeshAgent playerAgent;    
    private LineRenderer walkPath;

    // Objects
    private GameObject clickMarker, executeButton, buttonFrame, crosshair, enemy, playerUI;
    public GameObject projectile, firePoint;    

    // Audio
    private AudioSource audioUI;
    private AudioClip buttonClick;

    // Unit stats
    public static float mechSpeed = 3.5f;

    // Attack parameters
    public int burstSize;
    public float fireDelay;    
    public float fireRate;
    private UnitManager unitManager;

    // Start is called before the first frame update
    void Start()
    {
        camMain = Camera.main;
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

        // UI
        executeButton = GameObject.Find("ExecuteButton");
        audioUI = GameObject.Find("MainUI").GetComponent<AudioSource>();
        buttonFrame = executeButton.transform.GetChild(1).gameObject;
        playerUI = GameObject.Find("PlayerUI");
        buttonClick = GameObject.Find("AudioManager").GetComponent<AudioSourcesUI>().clickButton;

        // Set target
        crosshair = GameObject.Find("Crosshair");
        enemy = GameObject.Find("Enemy");
        unitManager = GetComponentInChildren<UnitManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Crosshair position
        crosshair.transform.position = camMain.WorldToScreenPoint(enemy.transform.position);
        playerUI.transform.position = camMain.WorldToScreenPoint(transform.position);

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

    // Start turn
    private void ExecuteOrder()
    {
        audioUI.PlayOneShot(buttonClick);
        buttonFrame.transform.DOScaleX(1.2f, 0.1f).SetLoops(4);

        this.Wait(MainMenu.buttonDelay, () => {
            buttonFrame.transform.DOScaleX(1f, 0.1f);            
        });

        playerAgent.speed = mechSpeed;
        gameManager.inAction = true;
    }
}
