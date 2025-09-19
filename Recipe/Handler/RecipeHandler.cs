using Industrica.Recipe.ExtendedRecipe;
using Industrica.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Recipe.Handler
{
    public static class RecipeHandler
    {
        public static bool TryGetRecipe<I, O, R>(IEnumerable<R> recipeStorage, I input, out R resultRecipe) where I : RecipeInput where O : RecipeOutput where R : Recipe<I, O>
        {
            foreach (R recipe in recipeStorage)
            {
                if (recipe.Test(input))
                {
                    resultRecipe = recipe;
                    return true;
                }
            }

            resultRecipe = null;
            return false;
        }

        public abstract record RecipeInput()
        {
            public abstract bool Valid();
        }

        public record RecipeItemInput(ItemsContainer ItemsContainer)
        {
            public bool Test(IEnumerable<ItemIngredient> ingredients)
            {
                foreach (ItemIngredient ingredient in ingredients)
                {
                    if (!ItemsContainer._items.TryGetValue(ingredient.TechType, out ItemsContainer.ItemGroup group))
                    {
                        return false;
                    }

                    List<InventoryItem> items = group.items;
                    if (items == null)
                    {
                        return false;
                    }

                    int itemLeeway = items.Count - ingredient.Amount;
                    if (itemLeeway < 0)
                    {
                        return false;
                    }

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (!ingredient.Test(items[i]))
                        {
                            itemLeeway--;
                            if (itemLeeway < 0)
                            {
                                return false;
                            }
                        }
                    }
                }

                return true;
            }

            public IEnumerable<Pickupable> GetUsedItems(IEnumerable<ItemIngredient> ingredients)
            {
                foreach (ItemIngredient ingredient in ingredients)
                {
                    if (!ItemsContainer._items.TryGetValue(ingredient.TechType, out ItemsContainer.ItemGroup group))
                    {
                        yield break;
                    }

                    List<InventoryItem> items = group.items;
                    int remainingMatches = ingredient.Amount;

                    for (int i = 0; i < items.Count; i++)
                    {
                        InventoryItem item = items[i];
                        if (ingredient.Test(item))
                        {
                            yield return item.item;

                            remainingMatches--;
                            if (remainingMatches <= 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        public abstract record RecipeOutput(TechType TechType, int Count)
        {
            public abstract TooltipIcon GetTooltipIcon();
        }

        public abstract record RecipeItemOutput(TechType TechType, int Count) : RecipeOutput(TechType, Count)
        {
            public override TooltipIcon GetTooltipIcon()
            {
                Sprite sprite = SpriteManager.Get(TechType);
                string name = TechType.AsString().Translate();

                if (Count == 1)
                {
                    return new TooltipIcon(sprite, "IndustricaItemByproductFormatOne".Translate(name));
                }

                return new TooltipIcon(sprite, "IndustricaItemByproductFormatMultiple".Translate(name, Count));
            }

            public void GetSizes(List<Vector2int> outputList)
            {
                outputList.AddRange(Enumerable.Repeat(TechData.GetItemSize(TechType), Count));
            }

            public IEnumerator RunOnItems(Action<Pickupable> callback)
            {
                TaskResult<GameObject> task = new();
                yield return SNUtil.TryGetItemPrefab(TechType, task);

                GameObject prefab = task.Get();
                if (prefab == null
                    || prefab.GetComponent<Pickupable>() == null)
                {
                    Plugin.Logger.LogWarning($"Could not find prefab of type {TechType} or {nameof(Pickupable)} on it, skipping parts of the output.");
                } else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        GameObject instance = UWE.Utils.InstantiateDeactivated(prefab);
                        callback(instance.GetComponent<Pickupable>());
                    }
                }
            }

            public IEnumerator RunOnInventoryItems(Action<InventoryItem> callback)
            {
                TaskResult<GameObject> task = new();
                yield return SNUtil.TryGetItemPrefab(TechType, task);

                GameObject prefab = task.Get();
                if (prefab == null
                    || prefab.GetComponent<Pickupable>() == null)
                {
                    Plugin.Logger.LogWarning($"Could not find prefab of type {TechType} or {nameof(Pickupable)} on it, skipping parts of the output.");
                    yield break;
                }
                else
                {
                    for (int i = 0; i < Count; i++)
                    {
                        GameObject instance = UWE.Utils.InstantiateDeactivated(prefab);
                        callback(new InventoryItem(instance.GetComponent<Pickupable>()));
                    }
                }
            }
        }

        public abstract record Recipe<I, O>(O[] Outputs, ExtendedRecipeData Data) where I : RecipeInput where O : RecipeOutput
        {
            public abstract bool Test(I input);
        }
    }
}
