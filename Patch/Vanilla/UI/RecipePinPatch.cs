using HarmonyLib;
using Industrica.Recipe;
using Industrica.Recipe.Handler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Emit;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_RecipeEntry), nameof(uGUI_RecipeEntry.UpdateIngredients))]
    public static class RecipePinPatch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Stloc_0))
                .ThrowIfInvalid($"Could not find {nameof(OpCodes.Stloc_0)}")
                .Advance(1)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_0))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, typeof(RecipePinPatch).GetMethod(nameof(CleanUpFakeIngredients), System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_0))
                .InstructionEnumeration();
        }

        private static ReadOnlyCollection<Ingredient> CleanUpFakeIngredients(ReadOnlyCollection<Ingredient> ingredients)
        {
            List<Ingredient> curated = new();
            foreach (Ingredient dirtyIngredient in ingredients)
            {
                if (!GeneralFakeIngredients.IsIngredientFake(dirtyIngredient.techType))
                {
                    curated.Add(dirtyIngredient);
                }
            }

            return new ReadOnlyCollection<Ingredient>(curated);
        }
    }
}
