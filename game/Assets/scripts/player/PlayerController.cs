using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;


public class PlayerController : MonoBehaviour
{
    private GameManager gameManager;

    // Objects
    private GameObject clickMarker, crosshair;
    private UnitManager playerManager;
    public GameObject target;

    // NavMesh
    [HideInInspector] public NavMeshAgent playerAgent;
    private LineRenderer walkPath;

    private float crosshairSize;
    private readonly float crosshairScale = 0.15f;
    public LayerMask IgnoreLayers;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        crosshair = GameObject.Find("Crosshair");

        // Navmesh setup        
        playerAgent = GetComponent<NavMeshAgent>();        
        walkPath = GetComponent<LineRenderer>();
        NavMesh.avoidancePredictionTime = 5;

        // Path
        clickMarker = GameObject.Find("ClickMarker");   
        walkPath.startWidth = 0.02f;
        walkPath.endWidth = 0.02f;
        walkPath.positionCount = 0;

        playerManager = GetComponentInChildren<UnitManager>();
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

        // Draw player path
        if (playerAgent.hasPath)
        {
            DrawPath();
        }

        // Dynamic crosshair
        crosshairSize = Mathf.Lerp(crosshairSize, crosshairScale + playerManager.spread / 2 + playerManager.moveSpeed / 10, Time.deltaTime * 3);
        crosshair.transform.localScale = crosshairSize * Vector3.one;
    }

    private void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~IgnoreLayers);

        if (isHit)
        {
            playerManager.SetDestination(hit.point, playerAgent);
        }       
    }

    // Draw player path
    public void DrawPath()
    {
        walkPath.positionCount = playerAgent.path.corners.Length;
        walkPath.SetPosition(0, transform.position);

        if (playerAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < playerAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new(playerAgent.path.corners[i].x, playerAgent.path.corners[i].y,
                    playerAgent.path.corners[i].z);
            walkPath.SetPosition(i, pointPosition);
            clickMarker.transform.position = new Vector3(pointPosition.x, 0.05f, pointPosition.z);
            clickMarker.transform.localScale = Vector3.zero;
            clickMarker.transform.DOScale(0.2f * Vector3.one , 0.2f);
        }

        clickMarker.transform.Rotate(new Vector3(0, 0, 50 * -Time.deltaTime));
    }

    public void Move()
    {
        playerAgent.speed = playerManager.moveSpeed + 0.5f;
    }

    public void EndMove()
    {
        playerManager.moveSpeed = 0.1f;
        playerManager.UpdateTimer();
    }
}
