using Industrica.Buildable.Electrical;
using Industrica.Buildable.Mining;
using Industrica.Buildable.Processing;
using Industrica.Buildable.Pump;
using Industrica.Buildable.Storage;
using Industrica.Buildable.Storage.Fluid;

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

            BuildableSmeltery.Register();
            BuildableCrusher.Register();

            BuildableElectricLever.Register();
            BuildableElectricOperator.Register();
            BuildableElectricSplitter.Register();
            BuildableElectricTimer.Register();

            BuildableWeighedItemLocker.Register();

            BuildableFluidTank.Register();
            BuildableFluidPump.Register();
        }
    }
}
