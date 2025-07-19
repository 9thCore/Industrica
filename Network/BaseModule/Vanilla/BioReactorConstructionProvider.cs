namespace Industrica.Network.BaseModule.Vanilla
{
    public class BioReactorConstructionProvider : BaseModuleProvider<BaseBioReactor>
    {
        public override float ConstructedAmount => Module._constructed;
        public override bool TryAddHandler(PortHandler handler, out PortHandler result)
        {
            BaseBioReactorGeometry geometry = Module.GetModel();
            if (geometry == null)
            {
                Plugin.Logger.LogError($"Could not fetch the geometry of {gameObject.name}??? Cannot apply construction patch.");
                result = default;
                return false;
            }

            result = handler.CopyTo(geometry.gameObject);
            return true;
        }
    }
}
