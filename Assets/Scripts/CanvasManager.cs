using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CanvasManager : MonoBehaviour
{
    public Texture2D texture { get; private set; }
    public Color[] colors { get; private set; }

    private SpriteRenderer spriteRend;

    void Awake()
    {
        spriteRend = GetComponent<SpriteRenderer>();

        var src = spriteRend.sprite.texture;

        texture = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false)
        {
            filterMode = FilterMode.Point,
            wrapMode = TextureWrapMode.Clamp
        };

        colors = src.GetPixels();
        texture.SetPixels(colors);
        texture.Apply();

        spriteRend.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void ApplyChanges()
    {
        texture.SetPixels(colors);
        texture.Apply();
    }
}
