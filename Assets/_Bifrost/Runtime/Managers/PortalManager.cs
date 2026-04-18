using UnityEngine;
using UnityEngine.InputSystem;

public class PortalController : MonoBehaviour
{
    [SerializeField] private Portal _portal;

    private InputAction action1;
    private InputAction action2;
    private InputAction action3;
    private InputAction action4;

    private void Awake()
    {
        action1 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/1");
        action2 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/2");
        action3 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/3");
        action4 = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/4");

        action1.performed += _ => TryOpen();
        action2.performed += _ => TryClose();
        action3.performed += _ => TryStabilize();
        action4.performed += _ => TryDestabilize();
    }

    private void OnEnable()
    {
        action1.Enable();
        action2.Enable();
        action3.Enable();
        action4.Enable();
    }

    private void OnDisable()
    {
        action1.Disable();
        action2.Disable();
        action3.Disable();
        action4.Disable();
    }

    private void TryOpen()
    {
        Debug.Log("Trying to open portal...");
        if (_portal == null)
        {
            Debug.LogError("Portal is NOT assigned!");
            return;
        }
        _portal.Open();
    }

    private void TryClose()
    {
        Debug.Log("Trying to close portal...");
        _portal.Close();
    }

    private void TryStabilize()
    {
        Debug.Log("Trying to stabilize portal...");
        _portal.Stabilize();
    }

    private void TryDestabilize()
    {
        Debug.Log("Trying to destabilize portal...");
        _portal.Destabilize();
    }
}