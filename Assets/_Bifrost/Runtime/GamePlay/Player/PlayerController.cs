using System.Collections;
using _Bifrost.Runtime.Managers.GamePlay;
using _Bifrost.Runtime.Portals;
using _Bifrost.UI.Controllers;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MainMenuController _mainMenuController;
    [SerializeField] private HUDController _hudController;
    
    public HUDController HUD => _hudController;
    
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _mouseSensitivity = 25f;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _jumpForce = 1.5f;
    [SerializeField] private float _gravity = -9.81f;
    public float gravityScale = 1f;

    [Header("Ice Effects")]
    [SerializeField] private LayerMask iceLayer;
    [SerializeField] private float iceControl = 0.2f; // контроль на льду
    [SerializeField] private float iceAcceleration = 2f;

    private bool _isOnIce;

    private Vector3 _currentVelocity;
    private float _yVelocity;
    
    private CharacterController _controller;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private Vector3 _externalVelocity;

    private float _xRotation = 0f;
    private InputSystem_Actions _inputActions;
    
    private InteractiveObject _current;
    private System.Action<PortalState> _currentPortalStateHandler;

    private void Start()
    {
        GameManager.Instance.RegisterPlayer(this);
        _controller = GetComponent<CharacterController>();
    }
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

    public void DisableInputForUI()
    {
        _inputActions.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void EnableInputForUI()
    {
        _inputActions.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            _yVelocity = -2f;
        }
        
        _isOnIce = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, iceLayer);

        Vector3 inputDir = transform.right * _moveInput.x + transform.forward * _moveInput.y;

        if (_isOnIce)
        {
            if (inputDir.sqrMagnitude > 0.01f)
            {
                // есть ввод → пытаемся управлять
                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    inputDir * _speed,
                    Time.deltaTime * iceAcceleration * iceControl
                );
            }
            else
            {
                // нет ввода → медленно тормозим (ИНЕРЦИЯ)
                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    Vector3.zero,
                    Time.deltaTime * 0.1f // ← главный параметр скольжения
                );
            }
        }
        else
        {
            if (inputDir.sqrMagnitude > 0.01f)
            {
                _currentVelocity = inputDir * _speed;
            }
            else
            {
                // мягкое торможение вне льда
                _currentVelocity = Vector3.Lerp(
                    _currentVelocity,
                    Vector3.zero,
                    Time.deltaTime * 10f
                );
            }
        }

        Vector3 move = _currentVelocity;
        move.y = _yVelocity + _externalVelocity.y;

        _yVelocity += _gravity * gravityScale * Time.deltaTime;

        // Применяем внешнюю силу от взрывов
        move += new Vector3(_externalVelocity.x, 0f, _externalVelocity.z);
        move.y = _yVelocity + _externalVelocity.y;

        if (_externalVelocity.sqrMagnitude > 0.01f)
        {
            _externalVelocity = Vector3.Lerp(_externalVelocity, Vector3.zero, Time.deltaTime * 4f);
        }

        _controller.Move(move * Time.deltaTime);
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
            _yVelocity = Mathf.Sqrt(_jumpForce * -2f * _gravity * gravityScale);
        }
    }
    
    public void ApplyKnockback(Vector3 force)
    {
        _externalVelocity += force;
    }

    private void UpdatePortalHint(ArchEnterPortal archPortal)
    {
        if (archPortal.Portal.state == PortalState.Closed)
            _hudController.ShowHint("E - открыть портал");
        else
            _hudController.ShowHint("E - закрыть портал");
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
                    // отписываемся от предыдущего события изменения состояния портала
                    UnsubscribeFromPortalStateChange();
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
                // отписываемся от события изменения состояния портала
                UnsubscribeFromPortalStateChange();
            }
        }
    }

    private void UnsubscribeFromPortalStateChange()
    {
        if (_currentPortalStateHandler != null)
        {
            // находим ArchEnterPortal и отписываемся от его события
            var archPortal = _current?.GetComponent<ArchEnterPortal>();
            if (archPortal?.Portal != null)
            {
                archPortal.Portal.OnStateChanged -= _currentPortalStateHandler;
            }
            _currentPortalStateHandler = null;
        }
    }

    private void CheckInteractiveObject()
    {
        var socket = _current.GetComponent<SocketPortal>();

        //  если это лунка
        if (socket != null)
        {
            // проверяем, что портал открыт
            if (socket.LinkedPortal != null && socket.LinkedPortal.state == PortalState.Closed)
            {
                _hudController.ShowError("Портал закрыт!");
                return;
            }

            if (socket.HasItem())
                _hudController.ShowHint("E - достать");
            else
                _hudController.ShowHint("E - вставить");

            return;
        }

        var archPortal = _current.GetComponent<ArchEnterPortal>();
        
        // если это портал для входа
        if (archPortal != null)
        {
            if (archPortal.Portal != null)
            {
                // подписываемся на изменение состояния портала
                _currentPortalStateHandler = (newState) => UpdatePortalHint(archPortal);
                archPortal.Portal.OnStateChanged += _currentPortalStateHandler;
                
                UpdatePortalHint(archPortal);
            }
            return;
        }

        //  если это обычный предмет (не кристалл)
        if (_current.GetComponent<Crystal>() == null)
        {
            if (_hudController.IsFullInventory)
                _hudController.ShowError("Инвентарь полон!");
            else
                _hudController.ShowHint("E - подобрать");
        }
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

        var archPortal = _current.GetComponent<ArchEnterPortal>();
        
        if (archPortal != null)
        {
            archPortal.Interact();
            return;
        }

        // обычный подбор (только не кристаллы)
        if (_current.GetComponent<Crystal>() == null)
        {
            PickUp(_current);
        }
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

        _hudController.AddToHotbar((IInventoryItem)obj);

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
        // проверяем, что портал открыт
        if (socket.LinkedPortal != null && socket.LinkedPortal.state == PortalState.Closed)
        {
            _hudController.ShowError("Портал закрыт!");
            return;
        }

        // если в лунке есть предмет → достаём
        var inside = socket.Take();

        if (inside != null)
        {
            bool added = _hudController.AddToHotbar((IInventoryItem)inside);

            if (!added)
            {
                // вернуть обратно, если нет места
                socket.Insert(inside);
                _hudController.ShowError("Инвентарь полон!");
                return;
            }

            inside.Hide();
            return;
        }

        // если пусто → пробуем вставить
        var item = _hudController.GetSelectedItem();

        if (item == null) return;

        var crystal = item as Crystal;
        if (crystal == null) return; // Может быть вставлен только Crystal

        if (!socket.CanInsert(crystal))
        {
            _hudController.ShowError("Неподходящий тип кристалла!");
            return;
        }

        _hudController.RemoveSelectedItem();
        socket.Insert(crystal);
    }
}