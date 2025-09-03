using Industrica.Utility;
using Nautilus.Crafting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using static Industrica.Recipe.Handler.SmelteryRecipeHandler.Recipe;

namespace Industrica.Recipe.Handler
{
    public static class RecipeHandler
    {
        public static bool TryGetRecipe<I, O, R>(IEnumerable<R> recipeStorage, I input, out R resultRecipe) where I : RecipeInput where O : RecipeOutput where R : MachineRecipe<I, O>
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

        public static bool Test(this Ingredient ingredient, InventoryItem inventoryItem)
        {
            return ingredient.techType == inventoryItem.techType;
        }

        public abstract record RecipeInput()
        {
            public abstract bool Valid();
        }

        public record RecipeItemInput(InventoryItem[] Items)
        {
            public bool Test(IEnumerable<Ingredient> ingredients)
            {
                foreach (Ingredient ingredient in ingredients)
                {
                    int amountUnaccountedFor = ingredient.amount;
                    foreach (InventoryItem item in Items)
                    {
                        if (ingredient.Test(item))
                        {
                            amountUnaccountedFor--;

                            if (amountUnaccountedFor == 0)
                            {
                                break;
                            }
                        }
                    }

                    if (amountUnaccountedFor > 0)
                    {
                        return false;
                    }
                }

                return true;
            }

            public IEnumerable<Pickupable> GetUsedItems(IEnumerable<Ingredient> ingredients)
            {
                foreach (Ingredient ingredient in ingredients)
                {
                    List<InventoryItem> usedItemsForThisIngredient = new();

                    int amountUnaccountedFor = ingredient.amount;
                    for (int i = Items.Count() - 1; i >= 0; i--)
                    {
                        InventoryItem item = Items[i];
                        if (ingredient.Test(item))
                        {
                            amountUnaccountedFor--;
                            usedItemsForThisIngredient.Add(item);

                            if (amountUnaccountedFor == 0)
                            {
                                foreach (InventoryItem used in usedItemsForThisIngredient)
                                {
                                    yield return used.item;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public abstract record RecipeOutput(TechType TechType, int Count)
        {
            public bool HasRoomIn(ItemsContainer container)
            {
                List<Vector2int> sizes = Enumerable.Repeat(TechData.GetItemSize(TechType), Count).ToList();
                return container.HasRoomFor(sizes);
            }

            public IEnumerator RunOnItems(Action<Pickupable> callback)
            {
                TaskResult<GameObject> task = new();
                yield return SNUtil.TryGetItemPrefab(TechType, task);

                GameObject prefab = task.Get();
                if (prefab == null)
                {
                    Plugin.Logger.LogWarning($"Could not find prefab of type {TechType} or {nameof(Pickupable)} on it, cannot create output.");
                    yield break;
                }

                for (int i = 0; i < Count; i++)
                {
                    GameObject instance = UWE.Utils.InstantiateDeactivated(prefab);
                    callback(instance.GetComponent<Pickupable>());
                }
            }
        }

        public abstract record MachineRecipe<I, O>(O[] Outputs, RecipeData Data) where I : RecipeInput where O : RecipeOutput
        {
            public abstract bool Test(I input);
        }
    }
}
