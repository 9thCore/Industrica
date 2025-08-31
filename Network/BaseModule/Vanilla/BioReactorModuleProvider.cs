namespace Industrica.Network.BaseModule.Vanilla
{
    public class BioReactorModuleProvider : BaseModuleProvider<BaseBioReactor>
    {
        public override float ConstructedAmount => module._constructed;
    }
}
