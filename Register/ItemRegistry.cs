using Industrica.Item.Filter;
using Industrica.Item.Mining.CoreSample;
using Industrica.Item.Tool;

namespace Industrica.Register
{
    public static class ItemRegistry
    {
        public static void Register()
        {
            ItemMultiTool.Register();
            ItemTechTypeFilter.Register();

            ItemCoreSampleEmpty.Register();
            ItemCoreSampleTitaniumCopper.Register();
        }
    }
}
