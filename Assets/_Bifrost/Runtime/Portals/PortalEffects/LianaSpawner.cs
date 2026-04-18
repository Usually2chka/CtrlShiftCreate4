using UnityEngine;

public class LianaSpawner : MonoBehaviour
{
    [Header("Настройки лианы")]
    [SerializeField] private Vector3 lianaScale = new Vector3(1, 1, 1);
    [SerializeField] private Vector3 lianaRotation = Vector3.zero;

    public Vector3 Position => transform.position;
    public Quaternion Rotation => Quaternion.Euler(lianaRotation);
    public Vector3 Scale => lianaScale;
}