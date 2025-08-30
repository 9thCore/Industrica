using Industrica.Item.Filter;
using Industrica.Item.Generic;
using Industrica.Item.Tool;

namespace Industrica.Register
{
    public static class ItemRegistry
    {
        public static void Register()
        {
            ItemMultiTool.Register();
            ItemTechTypeFilter.Register();

            ItemsBasic.Register();
        }
    }
}
