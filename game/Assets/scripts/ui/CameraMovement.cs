using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _cameraSpeed, _rotationSpeed;    
    [SerializeField] private GameObject _cameraFocus;
    [SerializeField] private int _mapSize;

    private PlayerInput _playerInput;
    private InputAction _move, _look, _rightClick, _middleClick, _rotate;    
    private float _rotation;
    private bool _once;    

    // Start is called before the first frame update
    void Awake()
    {
        _playerInput = new PlayerInput();
        transform.LookAt(_cameraFocus.transform);
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
    }

    void OnDisable()
    {
        _move.Disable();
        _look.Disable();
        _rightClick.Disable();
        _middleClick.Disable();
        _rotate.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!_once)
        {
            transform.LookAt(_cameraFocus.transform);
            _once = true;
        }*/

        if (_move.IsPressed() || _middleClick.IsPressed())
        {
            Move();
        }       


        if (_rightClick.IsPressed() || _rotate.IsPressed())
        {
            Rotate();
        }        
    }

    public void Move()
    {
        if (_move.IsPressed())
        {
            Vector2 moveDirection = _move.ReadValue<Vector2>();
            _cameraFocus.transform.position += new Vector3(moveDirection.x, 0 , moveDirection.y) * Time.deltaTime * _cameraSpeed;
        }

        if (_middleClick.IsPressed())
        {
            Vector2 moveDirection = _look.ReadValue<Vector2>();
            _cameraFocus.transform.position += new Vector3(moveDirection.x, 0 , moveDirection.y) * Time.deltaTime * _cameraSpeed / 4;
        }
        _cameraFocus.transform.position = new Vector3(Mathf.Clamp(_cameraFocus.transform.position.x, -_mapSize, _mapSize),
                _cameraFocus.transform.position.y,
                Mathf.Clamp(_cameraFocus.transform.position.z, -_mapSize, _mapSize));
    }

    private void Rotate()
    {
        if (_rightClick.IsPressed())
        {
            Vector3 look = _look.ReadValue<Vector2>();
            _rotation += look.x *_rotationSpeed;
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
}