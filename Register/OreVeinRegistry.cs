using Industrica.World.OreVein;

namespace Industrica.Register
{
    public static class OreVeinRegistry
    {
        public static void Register()
        {
            TitaniumCopperOreVein.Register();
            CopperSilverOreVein.Register();
            QuartzDiamondOreVein.Register();
            SilverGoldOreVein.Register();
            LeadUraniniteOreVein.Register();
            MagnetiteLithiumOreVein.Register();
            RubyKyaniteOreVein.Register();
        }
    }
}
