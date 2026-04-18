using System.Collections;
using UnityEngine;

public class DualTransformState : MonoBehaviour
{
    [Header("Состояния")]
    [SerializeField] private Transform stateA; // обычное
    [SerializeField] private Transform stateB; // эффект

    [Header("Настройки")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotateSpeed = 5f;

    private Coroutine currentRoutine;

    private void OnEnable()
    {
        SvartalfheimEffect.OnStateChanged += OnEffectChanged;
    }

    private void OnDisable()
    {
        SvartalfheimEffect.OnStateChanged -= OnEffectChanged;
    }

    private void OnEffectChanged(bool isActive)
    {
        Transform target = isActive ? stateB : stateA;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(MoveTo(target));
    }

    private IEnumerator MoveTo(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.01f ||
               Quaternion.Angle(transform.rotation, target.rotation) > 0.5f)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                target.position,
                Time.deltaTime * moveSpeed
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target.rotation,
                Time.deltaTime * rotateSpeed
            );

            yield return null;
        }

        // финальный snap (чтобы не дрожало)
        transform.position = target.position;
        transform.rotation = target.rotation;
    }
}