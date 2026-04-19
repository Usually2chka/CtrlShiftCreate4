using UnityEngine;
using System.Collections;

public class BreakableTile : MonoBehaviour
{
    private bool isDisabled = false;
    [SerializeField] private float delay = 0.1f;

    private void Start()
    {
        TileManager.Instance.Register(this);
    }

    private void OnEnable()
    {
        FloorSystem.OnStateChanged += OnSystemChanged;
    }

    private void OnDisable()
    {
        FloorSystem.OnStateChanged -= OnSystemChanged;
    }

    private void OnSystemChanged(bool active)
    {
        if (!active)
        {
            TileManager.Instance.RestoreAll();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"BreakableTile {gameObject.name} triggered by {other.gameObject.name}. FloorSystem active: {FloorSystem.IsActive}");
        if (!FloorSystem.IsActive) return;

        if (other.CompareTag("Player") && !isDisabled)
        {
            StartCoroutine(DisableAfterDelay());
        }
    }

    private IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(delay);

        // отключаем всю плитку (родителя)
        transform.parent.gameObject.SetActive(false);
    }

    public void ResetState()
    {
        isDisabled = false;
    }
}