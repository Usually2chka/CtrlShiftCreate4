using _Bifrost.Runtime.Portals;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct WorldTexture
{
    public string nameTexture;
    public WorldType worldType;
}
public class TextureSwitcher : MonoBehaviour
{
    public static TextureSwitcher Instance { get; private set; }
    private Renderer rend;
    [SerializeField] private float emissionStrength = 1f;
    [SerializeField] private WorldTexture[] worldTextures;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        GameObject tree = GameObject.FindWithTag("Tree");
        rend = tree.GetComponent<Renderer>();
        
        rend.material.SetFloat("_EmissionStrength", emissionStrength);
    }

    public void ShowLayer(WorldType worldType)
    {
        WorldTexture wt = System.Array.Find(worldTextures, wt => wt.worldType == worldType);
        if (wt.nameTexture != null)
        {
            rend.material.SetFloat(wt.nameTexture, 1f);
        }
    }

    public void HideLayer(WorldType worldType)
    {
        WorldTexture wt = System.Array.Find(worldTextures, wt => wt.worldType == worldType);
        if (wt.nameTexture != null)
        {
            rend.material.SetFloat(wt.nameTexture, 0f);
        }
    }
}