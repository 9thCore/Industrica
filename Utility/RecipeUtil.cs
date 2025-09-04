using Industrica.Recipe;
using Industrica.Recipe.Handler;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Industrica.Utility
{
    public static class RecipeUtil
    {
        public static void RegisterAlternativeRecipe(
            TechType result,
            int count,
            ExtendedRecipeData recipeData,
            IPrefabModifier[] modifiers = null,
            Action<CustomPrefab> callback = null)
        {
            recipeData.LinkedItems ??= new();
            recipeData.LinkedItems.AddRange(Enumerable.Repeat(result, count));

            recipeData.craftAmount = 0;

            if (CloneCount.TryGetValue(result, out int clones))
            {
                CloneCount[result]++;
            } else
            {
                clones = 0;
                CloneCount[result] = 1;
            }

            string classID = $"IndustricaAltRecipe_{result.AsString()}_{clones}";

            var prefab = new CustomPrefab(
                classId: classID,
                displayName: string.Empty,
                description: string.Empty,
                icon: SpriteManager.Get(result)
                );

            prefab.SetRecipe(recipeData);

            modifiers?.ForEach(modifier => modifier.Modify(prefab));

            prefab.SetGameObject(new CloneTemplate(prefab.Info, result));

            callback?.Invoke(prefab);

            TranslationRemapping.Add(prefab.Info.TechType, new RemapData(result, count));

            prefab.Register();

            // This has to be called after registry for some reason?
            CraftDataHandler.SetCraftingTime(prefab.Info.TechType, recipeData.CraftTime);
        }

        public static void Clear()
        {
            CloneCount.Clear();
        }

        public static string[] AsCraftPath(this string path)
        {
            return path.Split('/');
        }

        private static readonly Dictionary<TechType, int> CloneCount = new();
        public static readonly Dictionary<TechType, RemapData> TranslationRemapping = new();

        public record RemapData(TechType Result, int Count);

        public interface IPrefabModifier
        {
            void Modify(CustomPrefab prefab);
        }

        public record CrafterRecipe(CraftTree.Type Type, string[] Steps) : IPrefabModifier
        {
            public void Modify(CustomPrefab prefab)
            {
                CraftTreeHandler.AddCraftingNode(Type, prefab.Info.TechType, Steps);
            }
        }

        public record GroupAndCategory(TechGroup TechGroup, TechCategory TechCategory) : IPrefabModifier
        {
            public virtual void Modify(CustomPrefab prefab)
            {
                prefab.SetPdaGroupCategory(TechGroup, TechCategory);
            }
        }

        public record GroupAndCategoryBefore(TechGroup TechGroup, TechCategory TechCategory, TechType TechType) : GroupAndCategory(TechGroup, TechCategory)
        {
            public override void Modify(CustomPrefab prefab)
            {
                prefab.SetPdaGroupCategoryBefore(TechGroup, TechCategory, TechType);
            }
        }

        public record GroupAndCategoryAfter(TechGroup TechGroup, TechCategory TechCategory, TechType TechType) : GroupAndCategory(TechGroup, TechCategory)
        {
            public override void Modify(CustomPrefab prefab)
            {
                prefab.SetPdaGroupCategoryAfter(TechGroup, TechCategory, TechType);
            }
        }

        public record UnlockRequirement(TechType TechType, int FragmentCount = 1) : IPrefabModifier
        {
            public void Modify(CustomPrefab prefab)
            {
                prefab.SetUnlock(TechType, FragmentCount);
            }
        }
    }
}
