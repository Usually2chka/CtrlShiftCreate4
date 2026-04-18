using UnityEngine;

public class AsgardEffect : MonoBehaviour, IWorldEffect
{
    public float lowGravity = 0.1f;
    private float defaultGravity = 9.81f;

    public void Apply()
    {
        defaultGravity = Physics.gravity.y;
        Physics.gravity = new Vector3(0, lowGravity, 0);
    }

    public void Remove()
    {
        Physics.gravity = new Vector3(0, defaultGravity, 0);
    }
}