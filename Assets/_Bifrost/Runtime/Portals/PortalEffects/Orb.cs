using UnityEngine;

public class Orb
{
    public Transform SpawnPoint;
    public GameObject Instance;
    public Bomb Bomb; // 👈 ВАЖНО
    public bool Exploded;
    public float Timer;
}