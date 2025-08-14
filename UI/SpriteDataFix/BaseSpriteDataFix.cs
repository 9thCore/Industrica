using UnityEngine;

namespace Industrica.UI.SpriteDataFix
{
    // Because sprite data is not kept on prefab initialisation...
    public abstract class BaseSpriteDataFix : MonoBehaviour
    {
        public uGUI_Icon icon;

        public abstract Sprite Sprite { get; }

        public virtual void PerformAdditionalInit() { }

        public void Start()
        {
            icon.sprite = Sprite;
            GameObject.DestroyImmediate(this);
        }

        public void Init(Vector2 size)
        {
            icon.material = new Material(uGUI_ItemIcon.iconMaterial);

            RectTransformExtensions.Fit(icon.rectTransform, size.x, size.y, Sprite.rect.size.x, Sprite.rect.size.y, false);

            icon.material.SetVector(ShaderPropertyID._Size, size);

            Vector4 vector = new Vector4(-0.5f * size.x, -0.5f * size.y, size.x, size.y);
            icon.material.SetVector(ShaderPropertyID._FillRect, vector);

            PerformAdditionalInit();
        }
    }
}
