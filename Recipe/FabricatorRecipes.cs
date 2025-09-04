using Industrica.Item.Generic;
using Industrica.Recipe.Handler;
using Industrica.Utility;
using Nautilus.Crafting;
using Nautilus.Handlers;

namespace Industrica.Recipe
{
    public static class FabricatorRecipes
    {
        public const string BasicProcessing = "IndustricaBasicProcessing";

        public static readonly TechCategory BasicProcessingCategory = EnumHandler.AddEntry<TechCategory>(BasicProcessing)
            .RegisterToTechGroup(TechGroup.Resources);

        public static void Register()
        {
            CraftTreeHandler.AddTabNode(
                CraftTree.Type.Fabricator,
                BasicProcessing,
                string.Empty,
                SpriteManager.Get(TechType.LimestoneChunk),
                "Resources".AsCraftPath());

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.Titanium,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceTitaniumCopper.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceTitaniumCopper.TechType)
                });
        }
    }
}
