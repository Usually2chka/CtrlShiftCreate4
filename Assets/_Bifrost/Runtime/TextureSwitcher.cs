using _Bifrost.Runtime.Portals;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct WorldTexture
{
    public string nameTexture;
    public WorldType worldType;
    public GameObject spheres;
}
public class TextureSwitcher : MonoBehaviour
{
    public static TextureSwitcher Instance { get; private set; }
    private Renderer rend;
    [SerializeField] private float emissionStrength = 1f;
    [SerializeField] private float lerpStrength = 1f;
    [SerializeField] private WorldTexture[] worldTextures;
    private float oldEmission = 0f;

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

    void Update()
    {
        // if (Keyboard.current.digit6Key.wasPressedThisFrame)
        // {
        //     foreach (var wt in worldTextures)
        //     {
        //         rend.material.SetFloat(wt.nameTexture, lerpStrength);
        //     }
        // }
        if (emissionStrength != oldEmission)
        {
            rend.material.SetFloat("_EmissionStrength", emissionStrength);
            oldEmission = emissionStrength;
        }
    }

    public void ShowLayer(WorldType worldType)
    {
        WorldTexture wt = System.Array.Find(worldTextures, wt => wt.worldType == worldType);
        if (wt.nameTexture != null)
        {
            rend.material.SetFloat(wt.nameTexture, lerpStrength);
        }
        if (wt.spheres != null)
        {
            wt.spheres.SetActive(false);
        }
    }

    public void HideLayer(WorldType worldType)
    {
        WorldTexture wt = System.Array.Find(worldTextures, wt => wt.worldType == worldType);
        if (wt.nameTexture != null)
        {
            rend.material.SetFloat(wt.nameTexture, 0f);
        }
        if (wt.spheres != null)
        {
            wt.spheres.SetActive(true);
        }
    }
}