namespace Industrica.UI.SpriteDataFix
{
    public class BackgroundSpriteDataFix : BaseSpriteDataFix
    {
        public CraftData.BackgroundType type;
        public float radius = 0f;

        public BackgroundSpriteDataFix WithType(CraftData.BackgroundType type)
        {
            this.type = type;
            return this;
        }

        public BackgroundSpriteDataFix WithRadius(float radius)
        {
            this.radius = radius;
            return this;
        }

        public override void PerformAdditionalInit()
        {
            icon.material.EnableKeyword("SLICE_9_GRID");
            icon.material.SetFloat(ShaderPropertyID._Radius, radius);
        }

        public override Atlas.Sprite Sprite => SpriteManager.GetBackground(type);
    }
}
