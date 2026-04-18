using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    private List<BreakableTile> tiles = new List<BreakableTile>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
            RestoreAll();
        }
    }
    public void Register(BreakableTile tile)
    {
        if (!tiles.Contains(tile))
            tiles.Add(tile);
    }

    public void RestoreAll()
    {
        foreach (var tile in tiles)
        {
            tile.gameObject.transform.parent.gameObject.SetActive(true);
            tile.ResetState();
        }
    }
}