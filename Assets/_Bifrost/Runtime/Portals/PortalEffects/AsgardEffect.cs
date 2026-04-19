using UnityEngine;

public class AsgardEffect : MonoBehaviour, IWorldEffect
{
    private PlayerController _player;

    public float lowGravityScale = 0.1f;
    private float defaultGravityScale;

    public void Apply()
    {
        _player = GameManager.Instance.Player;
        defaultGravityScale = _player.gravityScale;
        _player.gravityScale = lowGravityScale;
    }

    public void Remove()
    {
        _player.gravityScale = defaultGravityScale;
    }
}