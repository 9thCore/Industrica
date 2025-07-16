namespace Industrica.Network.BaseModule.Vanilla
{
    public class NuclearReactorConstructionProvider : BaseModuleConstructionProvider<BaseNuclearReactor>
    {
        public override float ConstructedAmount => Module._constructed;
    }
}
