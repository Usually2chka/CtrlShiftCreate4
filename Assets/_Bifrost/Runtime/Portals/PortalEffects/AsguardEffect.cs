using UnityEngine;

public class AsgardEffect : MonoBehaviour, IWorldEffect
{
    [SerializeField] private PlayerController _player;

    public float lowGravityScale = 0.1f;
    private float defaultGravityScale;

    public void Apply()
    {
        defaultGravityScale = _player.gravityScale;
        _player.gravityScale = lowGravityScale;
    }

    public void Remove()
    {
        _player.gravityScale = defaultGravityScale;
    }
}