namespace Industrica.Network.BaseModule.Vanilla
{
    public class FiltrationMachineConstructionProvider : BaseModuleConstructionProvider<FiltrationMachine>
    {
        public override float ConstructedAmount => Module._constructed;
    }
}
