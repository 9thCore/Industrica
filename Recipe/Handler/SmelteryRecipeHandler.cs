using Industrica.Item.Generic.Builder;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace Industrica.Recipe.Handler
{
    public static class SmelteryRecipeHandler
    {
        public const string Smelting = "IndustricaSmelting";

        public static readonly TechCategory SmeltingCategory = EnumHandler.AddEntry<TechCategory>(Smelting)
            .RegisterToTechGroup(TechGroup.Resources);

        public static PrefabInfo LowHeat;
        public static PrefabInfo MediumHeat;
        public static PrefabInfo HighHeat;

        public static readonly List<Recipe> Recipes = new();

        public static void Register()
        {
            new EmptyItemBuilder("IndustricaHeatLevelLow")
                .WithIcon(SpriteManager.Get(TechType.CyclopsFireSuppressionModule))
                .Build(out LowHeat);

            new EmptyItemBuilder("IndustricaHeatLevelMedium")
                .WithIcon(SpriteManager.Get(TechType.CyclopsFireSuppressionModule))
                .Build(out MediumHeat);

            new EmptyItemBuilder("IndustricaHeatLevelHigh")
                .WithIcon(SpriteManager.Get(TechType.CyclopsFireSuppressionModule))
                .Build(out HighHeat);
        }

        public static void Register(
            Recipe.Output[] outputs,
            HeatLevel heatLevel,
            ExtendedRecipeData recipeData,
            List<RecipeUtil.IPrefabModifier> modifiers = null)
        {
            if (heatLevel == HeatLevel.None)
            {
                return;
            }

            Recipes.Add(new Recipe(outputs, heatLevel, recipeData));

            ExtendedRecipeData fakeRecipeData = recipeData.CreateCopy();

            switch (heatLevel)
            {
                case HeatLevel.Low:
                    fakeRecipeData.Ingredients.Add(new Ingredient(LowHeat.TechType, 1));
                    break;
                case HeatLevel.Medium:
                    fakeRecipeData.Ingredients.Add(new Ingredient(MediumHeat.TechType, 1));
                    break;
                case HeatLevel.High:
                    fakeRecipeData.Ingredients.Add(new Ingredient(HighHeat.TechType, 1));
                    break;
                default:
                    break;
            }

            modifiers ??= new();
            modifiers.Add(new RecipeUtil.GroupAndCategory(TechGroup.Resources, SmeltingCategory));
            RecipeUtil.RegisterAlternativeRecipe(outputs[0].TechType, outputs[0].Count, fakeRecipeData, modifiers.ToArray());
        }

        public static float GetSpeedMultiplier(HeatLevel currentHeatLevel, HeatLevel neededHeatLevel)
        {
            if (neededHeatLevel == HeatLevel.Low)
            {
                if (currentHeatLevel == HeatLevel.Medium)
                {
                    return MediumLowSpeed;
                } else if (currentHeatLevel == HeatLevel.High)
                {
                    return HighLowSpeed;
                }
            } else if (neededHeatLevel == HeatLevel.Medium)
            {
                if (currentHeatLevel == HeatLevel.High)
                {
                    return HighMediumSpeed;
                }
            }

            return DefaultSpeed;
        }

        public const float DefaultSpeed = 1f;
        public const float MediumLowSpeed = 1.2f;
        public const float HighLowSpeed = 1.5f;
        public const float HighMediumSpeed = HighLowSpeed / MediumLowSpeed;

        public record Recipe(Recipe.Output[] Outputs, HeatLevel RequiredHeatLevel, ExtendedRecipeData Data)
            : RecipeHandler.MachineRecipe<Recipe.Input, Recipe.Output>(Outputs, Data)
        {
            public override bool Test(Input input)
            {
                if (!input.Valid())
                {
                    return false;
                }

                return input.Item.Test(Data.Ingredients);
            }

            public IEnumerable<Pickupable> GetUsedItems(Input input)
            {
                return input.Item.GetUsedItems(Data.Ingredients);
            }

            public record Input(InventoryItem[] Items)
                : RecipeHandler.RecipeInput
            {
                public readonly RecipeHandler.RecipeItemInput Item = new(Items);

                public override bool Valid()
                {
                    return Items != null
                        && Items.Any();
                }
            }

            public record Output(TechType TechType, int Count)
                : RecipeHandler.RecipeOutput(TechType, Count);
        }

        public enum HeatLevel
        {
            None,
            Low,
            Medium,
            High
        }
    }
}
