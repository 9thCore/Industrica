using Industrica.Buildable.Electrical;
using Industrica.Buildable.Mining;
using Industrica.Buildable.Pump;
using Industrica.Buildable.Storage;

namespace Industrica.Register
{
    public static class BuildableRegistry
    {
        public static void Register()
        {
            BuildableItemPump.Register();

            BuildableBigItemLocker.Register();

            BuildabltemTechTypeFilterWriter.Register();
            BuildableFilterLocker.Register();

            BuildableInOutItemPassthrough.Register();

            BuildableCoreSampleDrill.Register();
            BuildableDrill.Register();

            BuildableElectricLever.Register();
            BuildableElectricOperator.Register();
            BuildableElectricSplitter.Register();
            BuildableElectricTimer.Register();

            BuildableWeighedItemLocker.Register();
        }
    }
}
