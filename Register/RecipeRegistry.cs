using Industrica.Recipe;
using Industrica.Recipe.Handler;
using Industrica.Utility;

namespace Industrica.Register
{
    public static class RecipeRegistry
    {
        public static void Register()
        {
            SmelteryRecipeHandler.Register();

            FabricatorRecipes.Register();
            SmelteryRecipes.Register();

            RecipeUtil.Clear();
        }
    }
}
