namespace Industrica.Network.BaseModule.Vanilla
{
    public class BioReactorConstructionProvider : BaseModuleConstructionProvider<BaseBioReactor>
    {
        public override float ConstructedAmount => Module._constructed;
    }
}
