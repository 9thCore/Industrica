namespace Industrica.UI.Inventory.Custom.Info.Image
{
    public class RotatingSpriteInfo : SpriteInfo
    {
        public float rotationSpeed;

        public override void Update()
        {
            image.transform.Rotate(0f, 0f, rotationSpeed * DayNightCycle.main.deltaTime);
        }
    }
}
