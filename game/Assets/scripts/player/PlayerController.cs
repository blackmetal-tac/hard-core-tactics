using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{ 
	public LayerMask IgnoreLayers;
	
	// NavMesh
    [HideInInspector] public NavMeshAgent PlayerAgent;
    private LineRenderer _walkPath;

    // Objects
	private GameManager _gameManager;
    private GameObject _clickMarker, _crosshair;
    private UnitManager _playerManager;

    private float _crosshairSize;
    private readonly float _crosshairScale = 0.15f;    
	
	void Awake()
	{
		_playerManager = GetComponentInChildren<UnitManager>();
		_playerManager.Target = GameObject.Find("EnemySquad").transform.Find("Enemy").gameObject;
	}

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _crosshair = GameObject.Find("Crosshair");		

        // Navmesh setup        
        PlayerAgent = GetComponent<NavMeshAgent>();
        NavMesh.avoidancePredictionTime = 5;
		_walkPath = GetComponent<LineRenderer>();

        // Path
        _clickMarker = GameObject.Find("ClickMarker");   
        _walkPath.startWidth = 0.02f;
        _walkPath.endWidth = 0.02f;
        _walkPath.positionCount = 0;       
    }

    // Update is called once per frame
    void FixedUpdate()
    {          
        // Mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (!_gameManager.inAction)
            {
                MoveToClick();
            }
        }

        // Draw player path
        if (PlayerAgent.hasPath)
        {
            DrawPath();
        }

        // Dynamic _crosshair
        _crosshairSize = Mathf.Lerp(_crosshairSize, _crosshairScale + _playerManager.Spread / 2 + _playerManager.MoveSpeed / 10, Time.fixedDeltaTime * 3);
        _crosshair.transform.localScale = _crosshairSize * Vector3.one;
    }

    private void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~IgnoreLayers);

        if (isHit)
        {
            _playerManager.SetDestination(hit.point, PlayerAgent);
        }       
    }

    // Draw player path
    public void DrawPath()
    {
        _walkPath.positionCount = PlayerAgent.path.corners.Length;
        _walkPath.SetPosition(0, transform.position);

        if (PlayerAgent.path.corners.Length < 2)
        {
            return;
        }

        for (int i = 1; i < PlayerAgent.path.corners.Length; i++)
        {
            Vector3 pointPosition = new(PlayerAgent.path.corners[i].x, PlayerAgent.path.corners[i].y,
                    PlayerAgent.path.corners[i].z);
            _walkPath.SetPosition(i, pointPosition);
            _clickMarker.transform.position = new Vector3(pointPosition.x, 0.05f, pointPosition.z);
            _clickMarker.transform.localScale = Vector3.zero;
            _clickMarker.transform.DOScale(0.2f * Vector3.one , 0.2f);
        }

        _clickMarker.transform.Rotate(new Vector3(0, 0, 50 * -Time.deltaTime));
    }

    public void Move()
    {
        PlayerAgent.speed = _playerManager.MoveSpeed + 0.5f;
    }

    public void EndMove()
    {
        _playerManager.MoveSpeed = 0.1f;
        _playerManager.UpdateOverheatTimer();
    }
}
