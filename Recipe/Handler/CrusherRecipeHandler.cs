using Industrica.Buildable.Processing;
using Industrica.Recipe.ExtendedRecipe;
using Industrica.Utility;
using Nautilus.Handlers;
using System.Collections.Generic;

namespace Industrica.Recipe.Handler
{
    public class CrusherRecipeHandler
    {
        public const string Crushing = "IndustricaCrushing";

        public static readonly TechCategory CrushingCategory = EnumHandler.AddEntry<TechCategory>(Crushing)
            .RegisterToTechGroup(TechGroup.Resources);

        public static readonly List<Recipe> Recipes = new();

        public static void Register(
            Recipe.Output[] outputs,
            ExtendedRecipeData recipeData,
            List<RecipeUtil.IPrefabModifier> modifiers = null)
        {
            Recipes.Add(new Recipe(outputs, recipeData));

            ExtendedRecipeData fakeRecipeData = recipeData.CreateCopy();

            modifiers ??= new();
            modifiers.Add(new RecipeUtil.GroupAndCategory(TechGroup.Resources, CrushingCategory));
            TechType recipe = RecipeUtil.RegisterAlternativeRecipe(outputs[0].TechType, outputs[0].Count, fakeRecipeData, modifiers.ToArray());

            RecipeDisplayUtil.SetRecipeInformation(recipe, new CrusherInformation(recipeData.CraftTime, outputs));
        }

        public record Recipe(Recipe.Output[] Outputs, ExtendedRecipeData Data)
            : RecipeHandler.Recipe<Recipe.Input, Recipe.Output>(Outputs, Data)
        {
            public override bool Test(Input input)
            {
                if (!input.Valid())
                {
                    return false;
                }

                return input.Item.Test(Data.ItemIngredients);
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
                : RecipeHandler.RecipeItemOutput(TechType, Count);
        }

        public class CrusherInformation : RecipeDisplayUtil.Information
        {
            public CrusherInformation(float craftTime, Recipe.Output[] outputs) : base(
                new TooltipIcon(
                    SpriteManager.Get(BuildableCrusher.Info.TechType),
                    BuildableCrusher.Info.TechType.AsString())
                , craftTime
                , outputs)
            { }
        }
    }
}
