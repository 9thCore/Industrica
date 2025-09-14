using Industrica.World.OreVein;

namespace Industrica.Register
{
    public static class OreVeinRegistry
    {
        public static void Register()
        {
            TitaniumCopperOreVein.Register();
            CopperSilverOreVein.Register();
            SilverGoldOreVein.Register();
            QuartzDiamondOreVein.Register();
            LeadUraniniteOreVein.Register();
            MagnetiteLithiumOreVein.Register();
            RubyKyaniteOreVein.Register();
            LithiumNickelOreVein.Register();
            CrashPowderSulfurOreVein.Register();
        }
    }
}
