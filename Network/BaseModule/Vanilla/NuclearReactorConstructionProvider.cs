namespace Industrica.Network.BaseModule.Vanilla
{
    public class NuclearReactorConstructionProvider : BaseModuleProvider<BaseNuclearReactor>
    {
        public override float ConstructedAmount => Module._constructed;
        public override void AddGeometryHandler(PortHandler handler)
        {
            BaseNuclearReactorGeometry geometry = Module.GetModel();
            if (geometry == null)
            {
                Plugin.Logger.LogError($"Could not fetch the geometry of {gameObject.name}??? Cannot apply construction patch.");
                return;
            }

            handler.CopyTo(geometry.gameObject);
        }
    }
}
