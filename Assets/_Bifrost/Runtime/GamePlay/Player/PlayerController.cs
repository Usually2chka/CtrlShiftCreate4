using _Bifrost.Runtime.Managers.GamePlay;
using UnityEngine;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _mouseSensitivity = 25f;
    [SerializeField] private Transform _playerCamera;

    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _xRotation = 0f;
    private InputSystem_Actions _inputActions;
    
    private InteractiveObject _current;
    private InteractiveObject _previousItem;
    private LayerMask _interactLayer;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();

        _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        Interact();
    }
    
    public void EnableInput()
    {
        _inputActions.Enable();
    }

    public void DisableInput()
    {
        _inputActions.Disable();
    }
    
    private void HandleMovement()
    {
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _controller.Move(move * _speed * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = _lookInput.x * _mouseSensitivity * Time.deltaTime;
        float mouseY = _lookInput.y * _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void Interact()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 5f))
        {
            var newObj = hit.collider.GetComponent<InteractiveObject>();

            if (newObj != _current)
            {
                if (_current != null)
                    _current.OnHoverExit();

                _current = newObj;

                if (_current != null)
                    _current.OnHoverEnter();
            }
        }
        else
        {
            if (_current != null)
            {
                _current.OnHoverExit();
                _current = null;
            }
        }
    }
}