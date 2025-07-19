namespace Industrica.Network.BaseModule.Vanilla
{
    public class NuclearReactorConstructionProvider : BaseModuleProvider<BaseNuclearReactor>
    {
        public override float ConstructedAmount => Module._constructed;
        public override bool TryAddHandler(PortHandler handler, out PortHandler result)
        {
            BaseNuclearReactorGeometry geometry = Module.GetModel();
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
