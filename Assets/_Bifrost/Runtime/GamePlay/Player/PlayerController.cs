using _Bifrost.Runtime.Managers.GamePlay;
using _Bifrost.UI.Controllers;
using UnityEngine;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private HUDController _hudController;
    
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _mouseSensitivity = 25f;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _jumpForce = 1.5f;
    [SerializeField] private float _gravity = -9.81f;
    
    private float _yVelocity;
    
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector2 _lookInput;

    private float _xRotation = 0f;
    private InputSystem_Actions _inputActions;
    
    private InteractiveObject _current;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += ctx => _moveInput = Vector2.zero;

        _inputActions.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();

        _inputActions.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
        
        _inputActions.Player.Jump.performed += ctx => Jump();
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
        TryToPickUp(_current);
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
        if (_controller.isGrounded && _yVelocity < 0)
        {
            _yVelocity = -2f; // "прилипание" к земле
        }

        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        _yVelocity += _gravity * Time.deltaTime;

        move.y = _yVelocity;

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
    
    private void Jump()
    {
        if (_controller.isGrounded)
        {
            _yVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity);
        }
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
                {
                    _current.OnHoverExit();
                    _hudController.HideHint();
                }

                _current = newObj;

                if (_current != null)
                {
                    _current.OnHoverEnter();
                    _hudController.ShowHint("E - Взаимодействовать");
                }
            }
        }
        else
        {
            if (_current != null)
            {
                _current.OnHoverExit();
                _current = null;
                _hudController.HideHint();
            }
        }
    }

    private void TryToPickUp(InteractiveObject obj)
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _current != null)
        {
            PickUp(obj);
        }
    }
    
    private void PickUp(InteractiveObject obj)
    {
        _hudController.AddToHotbar(obj);
        obj.gameObject.SetActive(false);
        
        _current = null;
        _hudController.HideHint();
    }
    
    private void ClearTarget()
    {
        _current = null;
        _hudController.HideHint();
    }
}