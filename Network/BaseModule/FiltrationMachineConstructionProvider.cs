namespace Industrica.Network.BaseModule
{
    public class FiltrationMachineConstructionProvider : BaseModuleConstructionProvider<FiltrationMachine>
    {
        public override float ConstructedAmount => Module._constructed;
    }
}
