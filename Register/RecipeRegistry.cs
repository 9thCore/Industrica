using Industrica.Recipe;
using Industrica.Utility;

namespace Industrica.Register
{
    public static class RecipeRegistry
    {
        public static void Register()
        {
            FabricatorRecipes.Register();

            RecipeUtil.Clear();
        }
    }
}
