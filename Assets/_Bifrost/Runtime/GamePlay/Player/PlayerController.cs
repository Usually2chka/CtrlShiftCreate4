using System.Collections;
using _Bifrost.Runtime.Managers.GamePlay;
using _Bifrost.UI.Controllers;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuController;
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

    public void EnableInput()
    {
        _inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void DisableInput()
    {
        _inputActions.Disable();
        HideHUD();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowMainMenu()
    {
        _mainMenuController.ShowSelf();
    }

    public void HideMainMenu()
    {
        _mainMenuController.HideSelf();
    }
    
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
        TryInteract();
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
                    CheckInteractiveObject();
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

    private void CheckInteractiveObject()
    {
        var socket = _current.GetComponent<SocketPortal>();

        //  если это лунка
        if (socket != null)
        {
            if (socket.HasItem())
                _hudController.ShowHint("E - достать");
            else
                _hudController.ShowHint("E - вставить");

            return;
        }

        //  если это обычный предмет
        if (_hudController.IsFullInventory)
            _hudController.ShowError("Инвентарь полон!");
        else
            _hudController.ShowHint("E - подобрать");
    }
    
    private void PickUp(InteractiveObject obj)
    {
        if (!_hudController.CanAddToHotbar())
        {
            _hudController.ShowError("Инвентарь полон!");
            return;
        }

        StartCoroutine(PickupAnimation(obj));
    }

    private void TryInteract()
    {
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;
        if (_current == null) return;

        var socket = _current.GetComponent<SocketPortal>();

        if (socket != null)
        {
            HandleSocket(socket);
            return;
        }

        // обычный подбор
        PickUp(_current);
    }
    
    private IEnumerator PickupAnimation(InteractiveObject obj)
    {
        obj.enabled = false;

        Vector3 startPos = obj.transform.position;
        Vector3 endPos = _playerCamera.position + _playerCamera.forward * 0.5f;

        float time = 0f;
        float duration = 0.2f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, time / duration);

            obj.transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        _hudController.AddToHotbar(obj);

        obj.gameObject.SetActive(false);

        _current = null;
        _hudController.HideHint();
    }
    
    private void HideHUD()
    {
        _hudController.HideSelf();
    }
    
    private void HandleSocket(SocketPortal socket)
    {
        // если в лунке есть предмет → достаём
        var inside = socket.Take();

        if (inside != null)
        {
            bool added = _hudController.AddToHotbar(inside);

            if (!added)
            {
                // вернуть обратно, если нет места
                socket.Insert(inside);
                _hudController.ShowError("Инвентарь полон!");
                return;
            }

            inside.gameObject.SetActive(false);
            return;
        }

        // если пусто → пробуем вставить
        var item = _hudController.GetSelectedItem();

        if (item == null) return;

        _hudController.RemoveSelectedItem();
        socket.Insert(item);
    }
}