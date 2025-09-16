using Industrica.Buildable.Processing;
using Industrica.Recipe.ExtendedRecipe;
using Industrica.Utility;
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

        public static readonly List<Recipe> Recipes = new();

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

            fakeRecipeData.HandleCatalysts();

            modifiers ??= new();
            modifiers.Add(new RecipeUtil.GroupAndCategory(TechGroup.Resources, SmeltingCategory));
            TechType recipe = RecipeUtil.RegisterAlternativeRecipe(outputs[0].TechType, outputs[0].Count, fakeRecipeData, modifiers.ToArray());

            RecipeDisplayUtil.SetRecipeInformation(recipe, new SmelteryInformation(recipeData.CraftTime, heatLevel));
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
            : RecipeHandler.Recipe<Recipe.Input, Recipe.Output>(Outputs, Data)
        {
            public override bool Test(Input input)
            {
                if (!input.Valid())
                {
                    return false;
                }

                if (Data.ItemCatalysts == null)
                {
                    return input.Item.Test(Data.ItemIngredients);
                }

                // This fails if catalysts and ingredients have a shared
                // ingredient, but whatever I'll just not have that I guess
                return input.Item.Test(Data.ItemCatalysts)
                    && input.Item.Test(Data.ItemIngredients);
            }

            public IEnumerable<Pickupable> GetUsedItems(Input input)
            {
                return input.Item.GetUsedItems(Data.ItemIngredients);
            }

            public record Input(ItemsContainer ItemsContainer)
                : RecipeHandler.RecipeInput
            {
                public readonly RecipeHandler.RecipeItemInput Item = new(ItemsContainer);

                public override bool Valid()
                {
                    return ItemsContainer.count > 0;
                }
            }

            public record Output(TechType TechType, int Count)
                : RecipeHandler.RecipeOutput(TechType, Count);
        }

        public class SmelteryInformation : RecipeDisplayUtil.Information
        {
            public SmelteryInformation(float craftTime, HeatLevel heat) : base(
                new TooltipIcon(
                    SpriteManager.Get(BuildableSmeltery.Info.TechType),
                    BuildableSmeltery.Info.TechType.AsString())
                , craftTime)
            {
                extraIcons.Add(new TooltipIcon(SpriteManager.Get(TechType.CyclopsFireSuppressionModule), $"IndustricaSmelteryHeat{heat}"));
            }
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
