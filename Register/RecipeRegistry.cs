using Industrica.Recipe;
using Industrica.Utility;

namespace Industrica.Register
{
    public static class RecipeRegistry
    {
        public static void Register()
        {
            FabricatorRecipes.Register();
            SmelteryRecipes.Register();

            RecipeUtil.Clear();
            ExtraIngredientHelper.Clear();
        }
    }
}
