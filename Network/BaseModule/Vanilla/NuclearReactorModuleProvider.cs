namespace Industrica.Network.BaseModule.Vanilla
{
    public class NuclearReactorModuleProvider : BaseModuleProvider<BaseNuclearReactor>
    {
        public override float ConstructedAmount => module._constructed;
    }
}
