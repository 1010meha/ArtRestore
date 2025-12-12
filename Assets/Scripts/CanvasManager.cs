using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CanvasManager : MonoBehaviour
{
    public Texture2D texture { get; private set; }
    public Color[] colors { get; private set; }

    private SpriteRenderer spriteRend;
    private bool isInitialized = false;

    void Awake()
    {
        InitializeCanvas();
    }

    /// <summary>
    /// Initialize or reinitialize the canvas with the current sprite
    /// </summary>
    public void InitializeCanvas()
    {
        spriteRend = GetComponent<SpriteRenderer>();
        
        if (spriteRend == null || spriteRend.sprite == null)
        {
            Debug.LogWarning("CanvasManager: SpriteRenderer or sprite is null!");
            return;
        }

        var src = spriteRend.sprite.texture;

        // Clean up old texture if it exists
        if (texture != null)
        {
            DestroyImmediate(texture);
        }

        texture = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        colors = src.GetPixels();
        texture.SetPixels(colors);
        texture.Apply();

        spriteRend.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        isInitialized = true;
    }

    /// <summary>
    /// Update the canvas with a new sprite
    /// </summary>
    public void SetSprite(Sprite newSprite)
    {
        if (spriteRend == null)
            spriteRend = GetComponent<SpriteRenderer>();
        
        if (spriteRend != null && newSprite != null)
        {
            spriteRend.sprite = newSprite;
            InitializeCanvas();
        }
    }

    public void ApplyChanges()
    {
        if (texture != null && colors != null)
        {
            texture.SetPixels(colors);
            texture.Apply();
        }
    }
}
