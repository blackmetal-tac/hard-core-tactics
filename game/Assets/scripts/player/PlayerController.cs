using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{ 
	[SerializeField] private LayerMask _ignoreLayers;
    private Camera _camMain;    
	
	// NavMesh
    [HideInInspector] public NavMeshAgent PlayerAgent;
    private LineRenderer _walkPath;

    // Objects
	private GameManager _gameManager;
    private GameObject _clickMarker, _crosshair;
    [HideInInspector] public UnitManager UnitManagerP;

    private float _crosshairSize;
    private readonly float _crosshairScale = 0.15f;    
	
	void Awake()
	{        
		UnitManagerP = GetComponentInChildren<UnitManager>();
		UnitManagerP.Target = GameObject.Find("EnemySquad").transform.Find("Enemy").GetComponentInChildren<UnitManager>();
	}

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _camMain = Camera.main;
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
    void Update()
    {          
        // Mouse click
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;                

            if (!_gameManager.InAction)
            {
                MoveToClick();
            }
        }

        // Draw player path
        if (PlayerAgent.hasPath)
        {
            DrawPath();
        }

        if (UnitManagerP.IsDead)
        {
            _walkPath.startWidth = 0;
            _walkPath.endWidth = 0;
            _clickMarker.transform.localScale = Vector3.zero;
        }

        if (!UnitManagerP.Target.IsDead)
        {
            // Dynamic _crosshair
            if (!_gameManager.InAction)
            {
                _crosshairSize = Mathf.Lerp(_crosshairSize, _crosshairScale + UnitManagerP.MoveSpeed 
                    * UnitManagerP.SpreadMult * 1.5f, Time.deltaTime * 3);
            }
            else
            {
                _crosshairSize = Mathf.Lerp(_crosshairSize, _crosshairScale + UnitManagerP.Spread / 8, Time.deltaTime * 3);                
            }
            _crosshair.transform.localScale = _crosshairSize * Vector3.one;

            // Crosshair position
            _crosshair.transform.position = _camMain.WorldToScreenPoint(UnitManagerP.Target.transform.position);
        }
        else
        {
            _crosshair.transform.localScale = Vector3.zero;
        }
    }

    private void MoveToClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool isHit = Physics.Raycast(ray, out RaycastHit hit, 100, ~_ignoreLayers);        

        if (isHit)
        {   
            _clickMarker.transform.position = hit.point;
            _clickMarker.transform.localScale = Vector3.zero;
            _clickMarker.transform.DOScale(0.2f * Vector3.one , 0.5f);
            UnitManagerP.SetDestination(hit.point, PlayerAgent);
        }       
    }

    // Draw player path
    public void DrawPath()
    {
        _walkPath.startWidth = 0.02f;
        _walkPath.endWidth = 0.02f;
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
            _clickMarker.transform.position = Vector3.MoveTowards(_clickMarker.transform.position, PlayerAgent.destination, 
            Time.deltaTime * Vector3.Distance(_clickMarker.transform.position, PlayerAgent.destination) * 4);
        }
        _clickMarker.transform.Rotate(new Vector3(0, 0, 50 * -Time.deltaTime));        
    }

    public void Move()
    {
        if (UnitManagerP != null || !UnitManagerP.Target.IsDead)
        {
            PlayerAgent.speed = UnitManagerP.MoveSpeed;   
            UnitManagerP.UnitShield.TurnOnOff();
            UnitManagerP.CoolingOverdrive();
        }
    }

    public void EndMove()
    {
        UnitManagerP.MoveSpeed = 0.1f;        
        UnitManagerP.EndMove();
    }
}
