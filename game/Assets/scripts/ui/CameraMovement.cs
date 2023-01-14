using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _cameraSpeed, _rotationSpeed, _zoomSpeed;
    [SerializeField] private int _mapSize;
    private GameManager _gameManager;
    private GameObject _cameraFocus;
    private PlayerInput _playerInput;
    private InputAction _move, _look, _rightClick, _middleClick, _rotate, _scrollWheel, _altAction;    
    private float _rotation, _angle;
    private bool _once;    

    // Start is called before the first frame update
    void Awake()
    {
        _playerInput = new PlayerInput();
        _cameraFocus = transform.parent.gameObject;
        transform.LookAt(_cameraFocus.transform);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnEnable()
    {
        _move = _playerInput.UI.Move;
        _move.Enable();

        _look = _playerInput.UI.Look;
        _look.Enable();

        _rightClick = _playerInput.UI.RightClick;
        _rightClick.Enable();

        _middleClick = _playerInput.UI.MiddleClick;
        _middleClick.Enable();

        _rotate = _playerInput.UI.Rotate;
        _rotate.Enable();

        _scrollWheel = _playerInput.UI.ScrollWheel;
        _scrollWheel.Enable();

        _altAction = _playerInput.UI.AltAction;
        _altAction.Enable();
    }

    void OnDisable()
    {
        _move.Disable();
        _look.Disable();
        _rightClick.Disable();
        _middleClick.Disable();
        _rotate.Disable();
        _scrollWheel.Disable();
        _altAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!_once)
        {
            transform.LookAt(_cameraFocus.transform);
            _once = true;
        }*/
        if (Time.time > _gameManager.LoadTime)
        {
            if (_scrollWheel.IsPressed() || _altAction.IsPressed())
            {
                Zoom();
            }

            if (_move.IsPressed() || _middleClick.IsPressed())
            {
                Move();
            } 

            if (_rightClick.IsPressed() || _rotate.IsPressed() || _altAction.IsPressed())
            {
                Rotate();
            }            
        }               
    }

    public void Move()
    {
        if (_move.IsPressed())
        {
            Vector2 moveDirection = _move.ReadValue<Vector2>();
            // Get camera position for relative movenemt
            Vector3 camDirection = transform.forward * moveDirection.y + transform.right * moveDirection.x;
            _cameraFocus.transform.position += new Vector3(camDirection.x, 0 , camDirection.z) * _cameraSpeed;            
        }

        if (_middleClick.IsPressed())
        {
            Vector2 moveDirection = _look.ReadValue<Vector2>();
            // Get camera position for relative movenemt
            Vector3 camDirection = transform.forward * moveDirection.y + transform.right * moveDirection.x;
            _cameraFocus.transform.position += new Vector3(camDirection.x, 0 , camDirection.z) * _cameraSpeed / 4;   
        }

        // Restrict movement to map size
        _cameraFocus.transform.position = new Vector3(Mathf.Clamp(_cameraFocus.transform.position.x, -_mapSize, _mapSize),
                _cameraFocus.transform.position.y,
                Mathf.Clamp(_cameraFocus.transform.position.z, -_mapSize, _mapSize));
    }

    private void Rotate()
    {
        if (_rightClick.IsPressed() || _altAction.IsPressed())
        {
            Vector3 look = _look.ReadValue<Vector2>();
            _rotation += look.x *_rotationSpeed / 2;
            _cameraFocus.transform.rotation = Quaternion.Euler(new Vector3(_cameraFocus.transform.rotation.x, _rotation, 
                _cameraFocus.transform.rotation.z));  
        }    

        if (_rotate.IsPressed())
        {            
            float look = _rotate.ReadValue<float>();  
            _rotation += look *_rotationSpeed;          
            _cameraFocus.transform.rotation = Quaternion.Euler(new Vector3(_cameraFocus.transform.rotation.x, _rotation, 
                _cameraFocus.transform.rotation.z));  
        }
    }

    private void Zoom()
    {                
        if (_scrollWheel.IsPressed())
        {
            Vector2 zoom = _scrollWheel.ReadValue<Vector2>();            
            transform.localPosition = new Vector3(transform.localPosition.x, 
                transform.localPosition.y + zoom.y *_zoomSpeed, 
                transform.localPosition.z - zoom.y *_zoomSpeed);            
        }

        if (_altAction.IsPressed())
        {
            Vector2 zoom = _look.ReadValue<Vector2>();       
            transform.localPosition = new Vector3(transform.localPosition.x, 
                transform.localPosition.y + zoom.y *_zoomSpeed * 2, 
                transform.localPosition.z - zoom.y *_zoomSpeed * 2);            
        }
        
        // Restrict zoom to map size
        transform.localPosition = new Vector3(transform.localPosition.x,
                Mathf.Clamp(transform.localPosition.y, 1, _mapSize), 
                Mathf.Clamp(transform.localPosition.z, -_mapSize, -1));      
    }
}
