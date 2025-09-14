using HarmonyLib;
using Industrica.Utility;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace Industrica.Patch.Vanilla.UI
{
    [HarmonyPatch(typeof(uGUI_Tooltip))]
    public static class GUITooltipPatches
    {
        [HarmonyPatch(nameof(uGUI_Tooltip.Set))]
        [HarmonyPostfix]
        public static void Set_Postfix(ITooltip tooltip)
        {
            uGUI_Tooltip.main.gameObject.EnsureComponent<GUIRecipeInformation>();

            if (tooltip is uGUI_RecipeEntry recipeEntry)
            {
                GUIRecipeInformation.Instance.Set(recipeEntry.techType);
            } else if (tooltip is uGUI_BlueprintEntry blueprintEntry
                && blueprintEntry.TryGetComponent(out GUIBlueprintEntryPatch.Holder blueprintHolder))
            {
                GUIRecipeInformation.Instance.Set(blueprintHolder.techType);
            } else
            {
                GUIRecipeInformation.Instance.Reset();
            }
        }

        [HarmonyPatch(nameof(uGUI_Tooltip.OnLayout))]
        [HarmonyPrefix]
        public static void OnLayout_Prefix(uGUI_Tooltip __instance)
        {
            if (GUIRecipeInformation.Instance == null)
            {
                return;
            }

            GUIRecipeInformation.Instance.AddExtraIcons(__instance);
        }

        [HarmonyPatch(nameof(uGUI_Tooltip.OnLayout))]
        [HarmonyPostfix]
        public static void OnLayout_Postfix(uGUI_Tooltip __instance)
        {
            if (GUIRecipeInformation.Instance == null)
            {
                return;
            }

            
            GUIRecipeInformation.Instance.RemoveExtraIcons(__instance);
        }

        [HarmonyPatch(nameof(uGUI_Tooltip.OnLayout))]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> OnLayout_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            string representation = $"{nameof(uGUI_Tooltip)}.{nameof(uGUI_Tooltip.OnLayout)}";

            CodeMatch loadThis = new(OpCodes.Ldarg_0);

            FieldInfo iconCanvas = typeof(uGUI_Tooltip).GetField(nameof(uGUI_Tooltip.iconCanvas));
            CodeMatch loadIconCanvas = new(code => code.LoadsField(iconCanvas));

            Type layoutGroup = typeof(LayoutGroup);

            MethodInfo calculateLayoutInputHorizontal = layoutGroup.GetMethod(nameof(LayoutGroup.CalculateLayoutInputHorizontal));
            MethodInfo setLayoutHorizontal = layoutGroup.GetMethod(nameof(LayoutGroup.SetLayoutHorizontal));
            MethodInfo calculateLayoutInputVertical = layoutGroup.GetMethod(nameof(LayoutGroup.CalculateLayoutInputVertical));
            MethodInfo setLayoutVertical = layoutGroup.GetMethod(nameof(LayoutGroup.SetLayoutVertical));

            BindingFlags getNewWidthFlags = BindingFlags.Static | BindingFlags.Public;
            MethodInfo getNewWidth = typeof(GUIRecipeInformation).GetMethod(nameof(GUIRecipeInformation.IfExistsGetNewWidth), getNewWidthFlags);

            BindingFlags floatMinFlags = BindingFlags.Static | BindingFlags.Public;
            MethodInfo floatMin = typeof(Mathf).GetMethod(nameof(Mathf.Min), floatMinFlags, null, new Type[] { typeof(float), typeof(float) }, null);

            CodeMatch loadRunningTotalWidth = new(OpCodes.Ldloc_3);
            CodeMatch pushFloat = new(OpCodes.Ldc_R4);
            CodeMatch getMinimum = new(code => code.Calls(floatMin));
            CodeMatch storeRunningTotalWidth = new(OpCodes.Stloc_3);

            const int runningTotalHeightIndex = 7;
            const int preferredHeightIndex = 5;

            CodeMatch loadRunningTotalHeight = new(code => code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder { LocalIndex: runningTotalHeightIndex });
            CodeMatch loadpreferredHeight = new(code => code.opcode == OpCodes.Ldloc_S && code.operand is LocalBuilder { LocalIndex: preferredHeightIndex });
            CodeMatch storeRunningTotalHeight = new(code => code.opcode == OpCodes.Stloc_S && code.operand is LocalBuilder { LocalIndex: runningTotalHeightIndex });

            BindingFlags getNewHeightFlags = BindingFlags.Static | BindingFlags.Public;
            MethodInfo getNewHeight = typeof(GUIRecipeInformation).GetMethod(nameof(GUIRecipeInformation.IfExistsPositionLayoutAndGetNewHeight), getNewHeightFlags);

            return new CodeMatcher(instructions)
                .MatchForward(false, loadThis, loadIconCanvas, new CodeMatch(code => code.Calls(calculateLayoutInputHorizontal)))
                .ThrowIfInvalid($"Cannot find where to inject {nameof(GUIRecipeInformation.CalculateLayoutInputHorizontal)} in {representation}")
                .InsertAndAdvance(CodeInstruction.Call(() => GUIRecipeInformation.IfExistsCalculateLayoutInputHorizontal()))

                .MatchForward(false, loadRunningTotalWidth, pushFloat, getMinimum, storeRunningTotalWidth)
                .ThrowIfInvalid($"Cannot find where to inject {nameof(GUIRecipeInformation.GetNewWidth)} in {representation}")
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, getNewWidth))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_3))

                .MatchForward(false, loadThis, loadIconCanvas, new CodeMatch(code => code.Calls(setLayoutHorizontal)))
                .ThrowIfInvalid($"Cannot find where to inject {nameof(GUIRecipeInformation.SetLayoutHorizontal)} in {representation}")
                .Advance(3)
                .InsertAndAdvance(CodeInstruction.Call(() => GUIRecipeInformation.IfExistsSetLayoutHorizontal()))

                .MatchForward(false, loadThis, loadIconCanvas, new CodeMatch(code => code.Calls(calculateLayoutInputVertical)))
                .ThrowIfInvalid($"Cannot find where to inject {nameof(GUIRecipeInformation.CalculateLayoutInputVertical)} in {representation}")
                .InsertAndAdvance(CodeInstruction.Call(() => GUIRecipeInformation.IfExistsCalculateLayoutInputVertical()))

                .MatchForward(false, loadThis, loadIconCanvas, new CodeMatch(code => code.Calls(setLayoutVertical)))
                .ThrowIfInvalid($"Cannot find where to inject {nameof(GUIRecipeInformation.SetLayoutVertical)} in {representation}")
                .Advance(3)
                .InsertAndAdvance(CodeInstruction.Call(() => GUIRecipeInformation.IfExistsSetLayoutVertical()))

                .MatchForward(false, loadRunningTotalHeight, loadpreferredHeight, new(OpCodes.Add), storeRunningTotalHeight)
                .ThrowIfInvalid($"Cannot find where to inject {nameof(GUIRecipeInformation.PositionLayoutAndGetNewHeight)} in {representation}")
                .Advance(4)
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_3))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldloc_S, runningTotalHeightIndex))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Call, getNewHeight))
                .InsertAndAdvance(new CodeInstruction(OpCodes.Stloc_S, runningTotalHeightIndex))

                .InstructionEnumeration();
        }

        [HarmonyPatch(nameof(uGUI_Tooltip.UpdatePosition))]
        [HarmonyPostfix]
        public static void UpdatePosition_Postfix()
        {
            if (GUIRecipeInformation.Instance == null)
            {
                return;
            }

            GUIRecipeInformation.Instance.UpdatePosition();
        }
    }
}
