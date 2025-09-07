using System;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Utility
{
    public static class RecipeDisplayUtil
    {
        public static bool TryGet(TechType techType, out Information information)
        {
            return Recipes.TryGetValue(techType, out information);
        }

        public static void SetRecipeInformation(TechType techType, Information information)
        {
            Recipes[techType] = information;
        }

        private static readonly Dictionary<TechType, Information> Recipes = new();

        public abstract class Information
        {
            public readonly TooltipIcon machine;
            public readonly float craftTime;
            public readonly List<TooltipIcon> extraIcons = new();

            public Information(TooltipIcon machine, float craftTime)
            {
                this.machine = machine;
                this.craftTime = craftTime;
            }
        }
    }
}
