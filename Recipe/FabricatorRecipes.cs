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

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.Copper,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceCopperSilver.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceCopperSilver.TechType)
                });

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.Quartz,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceQuartzDiamond.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceQuartzDiamond.TechType)
                });

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.Silver,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceSilverGold.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceSilverGold.TechType)
                });

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.Lead,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceLeadUraninite.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceLeadUraninite.TechType)
                });

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.Magnetite,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceMagnetiteLithium.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceMagnetiteLithium.TechType)
                });

            RecipeUtil.RegisterAlternativeRecipe(
                result: TechType.AluminumOxide,
                count: 1,
                recipeData: new()
                {
                    Ingredients =
                    {
                        new Ingredient(ItemsBasic.OreVeinResourceRubyKyanite.TechType, 1)
                    },
                    CraftTime = 2f
                },
                modifiers: new RecipeUtil.IPrefabModifier[] {
                    new RecipeUtil.CrafterRecipe(CraftTree.Type.Fabricator, $"Resources/{BasicProcessing}".AsCraftPath()),
                    new RecipeUtil.GroupAndCategory(TechGroup.Resources, BasicProcessingCategory),
                    new RecipeUtil.UnlockRequirement(ItemsBasic.OreVeinResourceRubyKyanite.TechType)
                });
        }
    }
}
