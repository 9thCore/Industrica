using Industrica.Recipe;
using Industrica.Utility;
using Nautilus.Handlers;
using System.Collections;
using UnityEngine;

namespace Industrica.Register
{
    public static class RecipeRegistry
    {
        private static bool alreadyRegistered = false;

        public static IEnumerator Register(WaitScreenHandler.WaitScreenTask task)
        {
            if (alreadyRegistered)
            {
                yield break;
            }
            alreadyRegistered = true;

            yield return FabricatorRecipes.Register(task);
            yield return SmelteryRecipes.Register(task);

            task.Status = "IndustricaLoading_Localization".Translate();
            yield return null;

            LocalizationUtil.ApplyLocalizationData(Language.main);

            RecipeUtil.Clear();
            ExtraIngredientHelper.Clear();
        }
    }
}
