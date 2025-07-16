namespace Industrica.Network.BaseModule
{
    public class NuclearReactorConstructionProvider : BaseModuleConstructionProvider<BaseNuclearReactor>
    {
        public override float ConstructedAmount => Module._constructed;
    }
}
