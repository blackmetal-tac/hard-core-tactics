using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float _cameraSpeed, _rotationSpeed;    
    [SerializeField] private GameObject _cameraFocus;

    private PlayerInput _playerInput;
    private InputAction _move, _look, _rightClick;    
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
    }

    void OnDisable()
    {
        _move.Disable();
        _look.Disable();
        _rightClick.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!_once)
        {
            transform.LookAt(_cameraFocus.transform);
            _once = true;
        }*/

        Move();

        

        if (_rightClick.IsPressed())
        {
            Rotate();
        }        
    }

    public void Move()
    {
        Vector2 moveDirection = _move.ReadValue<Vector2>();
        _cameraFocus.transform.position += new Vector3(moveDirection.x, 0 , moveDirection.y) * Time.deltaTime * _cameraSpeed;
    }

    public void MouseMove()
    {

    }

    private void Rotate()
    {
        Vector3 look = _look.ReadValue<Vector2>();
        _rotation += look.x *_rotationSpeed;
        _cameraFocus.transform.rotation = Quaternion.Euler(new Vector3(_cameraFocus.transform.rotation.x, _rotation, 
            _cameraFocus.transform.rotation.z));     
    }
}
