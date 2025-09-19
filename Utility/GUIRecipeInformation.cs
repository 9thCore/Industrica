using Industrica.Recipe.Handler;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Utility
{
    public class GUIRecipeInformation : MonoBehaviour
    {
        public static GUIRecipeInformation Instance { get; private set; }

        private RecipeDisplayUtil.Information previousInformation;
        private RectTransform rectTransform;
        private FlexibleGridLayout machineInfoLayout;
        private FlexibleGridLayout byproductsLayout;
        private uGUI_TooltipIcon machine;
        private uGUI_TooltipIcon craftTimeDisplay;
        private List<uGUI_TooltipIcon> extraIcons;
        private List<uGUI_TooltipIcon> byproductIcons;
        private TechType techType;
        
        private void Awake()
        {
            Instance = this;
            rectTransform = GetComponent<RectTransform>();
        }

        public void Set(TechType techType)
        {
            if (this.techType == techType)
            {
                return;
            }

            if (RecipeDisplayUtil.TryGet(techType, out RecipeDisplayUtil.Information information))
            {
                SetInformation(information);
            }
            else
            {
                Reset();
            }
        }

        public void Reset()
        {
            previousInformation = null;
            techType = default;
            ResetMachine();
            ResetCraftTime();
            ResetIcons();
            ResetByproducts();
        }

        private void SetInformation(RecipeDisplayUtil.Information information)
        {
            if (previousInformation == information)
            {
                return;
            }

            OnUpdate(information);
            previousInformation = information;
        }

        private void OnUpdate(RecipeDisplayUtil.Information information)
        {
            if (information.machine != default)
            {
                SetMachine(information.machine);
            }

            if (information.craftTime != default)
            {
                SetCraftTime(information.craftTime);
            }

            if (information.extraIcons.Count != 0)
            {
                SetIcons(information.extraIcons);
            } else
            {
                ResetIcons();
            }

            if (information.byproducts != null)
            {
                SetByproducts(information.byproducts);
            } else
            {
                ResetByproducts();
            }
        }

        private Transform GetOrCreateMachineInfoLayout()
        {
            if (machineInfoLayout == null)
            {
                GameObject layout = uGUI_Tooltip.main.iconCanvas.transform.parent.gameObject.CreateChild($"Industrica{nameof(machineInfoLayout)}");
                layout.layer = LayerID.UI;
                machineInfoLayout = layout.AddComponent<FlexibleGridLayout>();
                machineInfoLayout.horizontalAlignment = FlexibleGridLayout.HorizontalAlignment.Center;
                machineInfoLayout.rectTransform.anchorMin = Vector2.up;
                machineInfoLayout.rectTransform.anchorMax = Vector2.up;
            }

            return machineInfoLayout.transform;
        }

        private Transform GetOrCreateByproductsLayout()
        {
            if (byproductsLayout == null)
            {
                GameObject layout = uGUI_Tooltip.main.iconCanvas.transform.parent.gameObject.CreateChild($"Industrica{nameof(byproductsLayout)}");
                layout.layer = LayerID.UI;
                byproductsLayout = layout.AddComponent<FlexibleGridLayout>();
                byproductsLayout.horizontalAlignment = FlexibleGridLayout.HorizontalAlignment.Center;
                byproductsLayout.rectTransform.anchorMin = Vector2.up;
                byproductsLayout.rectTransform.anchorMax = Vector2.up;
            }

            return byproductsLayout.transform;
        }

        private uGUI_TooltipIcon CreateTooltipIcon(Transform parent, float width, float height)
        {
            uGUI_TooltipIcon component = UWE.Utils.InstantiateDeactivated(uGUI_Tooltip.main.prefabIconEntry.gameObject).GetComponent<uGUI_TooltipIcon>();
            component.rectTransform.SetParent(parent, false);
            component.SetSize(width, height);
            return component;
        }

        private void SetMachine(TooltipIcon icon)
        {
            if (machine == null)
            {
                machine = CreateTooltipIcon(GetOrCreateMachineInfoLayout(), IconSize, IconSize);
            }

            machine.gameObject.SetActive(true);
            machine.SetIcon(icon.sprite);
            machine.SetText(LocalizationUtil.MachineCraftKey.Translate(icon.text.Translate()));
        }

        private void SetCraftTime(float craftTime)
        {
            if (craftTimeDisplay == null)
            {
                craftTimeDisplay = CreateTooltipIcon(GetOrCreateMachineInfoLayout(), IconSize, IconSize);
                craftTimeDisplay.SetIcon(SpriteManager.Get(TechType.MapRoomUpgradeScanSpeed));
            }

            craftTimeDisplay.gameObject.SetActive(true);
            craftTimeDisplay.SetText(LocalizationUtil.TimeKey.Translate(craftTime));
        }

        private void SetIcons(List<TooltipIcon> icons)
        {
            extraIcons ??= new();

            while (extraIcons.Count < icons.Count)
            {
                extraIcons.Add(CreateTooltipIcon(GetOrCreateMachineInfoLayout(), IconSize, IconSize));
            }

            int i;
            for (i = 0; i < icons.Count; i++)
            {
                uGUI_TooltipIcon extraIcon = extraIcons[i];
                TooltipIcon icon = icons[i];
                extraIcon.SetIcon(icon.sprite);
                extraIcon.SetText(icon.text.Translate());
                extraIcon.gameObject.SetActive(true);
            }

            for (; i < extraIcons.Count; i++)
            {
                extraIcons[i].gameObject.SetActive(false);
            }
        }

        private void SetByproducts(RecipeHandler.RecipeOutput[] byproducts)
        {
            byproductIcons ??= new();

            while (byproductIcons.Count < byproducts.Length)
            {
                byproductIcons.Add(CreateTooltipIcon(GetOrCreateByproductsLayout(), ByproductIconSize, ByproductIconSize));
            }

            int i;
            for (i = 0; i < byproducts.Length; i++)
            {
                uGUI_TooltipIcon byproduct = byproductIcons[i];
                TooltipIcon icon = byproducts[i].GetTooltipIcon();
                byproduct.SetIcon(icon.sprite);
                byproduct.SetText(icon.text);
                byproduct.gameObject.SetActive(true);
            }

            for (; i < byproductIcons.Count; i++)
            {
                byproductIcons[i].gameObject.SetActive(false);
            }
        }

        private void ResetMachine()
        {
            if (machine == null)
            {
                return;
            }

            machine.gameObject.SetActive(false);
        }

        private void ResetCraftTime()
        {
            if (craftTimeDisplay == null)
            {
                return;
            }

            craftTimeDisplay.gameObject.SetActive(false);
        }

        private void ResetIcons()
        {
            if (extraIcons == null
                || extraIcons.Count == 0)
            {
                return;
            }

            foreach (uGUI_TooltipIcon icon in extraIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        private void ResetByproducts()
        {
            if (byproductIcons == null
                || byproductIcons.Count == 0)
            {
                return;
            }

            foreach (uGUI_TooltipIcon icon in byproductIcons)
            {
                icon.gameObject.SetActive(false);
            }
        }

        public void AddExtraIcons(uGUI_Tooltip instance)
        {
            if (machine != null)
            {
                instance.icons.Add(machine);
            }

            if (craftTimeDisplay != null)
            {
                instance.icons.Add(craftTimeDisplay);
            }

            if (extraIcons != null
                && extraIcons.Count > 0)
            {
                instance.icons.AddRange(extraIcons);
            }

            if (byproductIcons != null
                && byproductIcons.Count > 0)
            {
                instance.icons.AddRange(byproductIcons);
            }
        }

        public void RemoveExtraIcons(uGUI_Tooltip instance)
        {
            if (machine != null)
            {
                instance.icons.Remove(machine);
            }

            if (craftTimeDisplay != null)
            {
                instance.icons.Remove(craftTimeDisplay);
            }

            if (extraIcons != null
                && extraIcons.Count > 0)
            {
                foreach (uGUI_TooltipIcon icon in extraIcons)
                {
                    instance.icons.Remove(icon);
                }
            }

            if (byproductIcons != null
                && byproductIcons.Count > 0)
            {
                foreach (uGUI_TooltipIcon icon in byproductIcons)
                {
                    instance.icons.Remove(icon);
                }
            }
        }

        public void CalculateLayoutInputHorizontal()
        {
            float width = rectTransform.sizeDelta.x;

            if (machineInfoLayout != null)
            {
                machineInfoLayout.CalculateLayoutInputHorizontal();
                width = Mathf.Max(width, machineInfoLayout.preferredWidth + MachineInfoLayoutPadding * 2f);
            }

            if (byproductsLayout != null)
            {
                byproductsLayout.CalculateLayoutInputHorizontal();
                width = Mathf.Max(width, byproductsLayout.preferredWidth + ByproductLayoutPadding * 2f);
                byproductsLayout.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }

            if (machineInfoLayout != null)
            {
                machineInfoLayout.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            }
        }

        public static void IfExistsCalculateLayoutInputHorizontal()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.CalculateLayoutInputHorizontal();
        }

        public void SetLayoutHorizontal()
        {
            if (machineInfoLayout != null)
            {
                machineInfoLayout.SetLayoutHorizontal();
            }
            
            if (byproductsLayout != null)
            {
                byproductsLayout.SetLayoutHorizontal();
            }
        }

        public static void IfExistsSetLayoutHorizontal()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.SetLayoutHorizontal();
        }

        public void CalculateLayoutInputVertical()
        {
            if (machineInfoLayout != null)
            {
                machineInfoLayout.CalculateLayoutInputVertical();
            }
            
            if (byproductsLayout != null)
            {
                byproductsLayout.CalculateLayoutInputVertical();
            }
        }

        public static void IfExistsCalculateLayoutInputVertical()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.CalculateLayoutInputVertical();
        }

        public void SetLayoutVertical()
        {
            if (machineInfoLayout != null)
            {
                machineInfoLayout.SetLayoutVertical();
            }
            
            if (byproductsLayout != null)
            {
                byproductsLayout.SetLayoutVertical();
            }
        }

        public static void IfExistsSetLayoutVertical()
        {
            if (Instance == null)
            {
                return;
            }

            Instance.SetLayoutVertical();
        }

        public float GetNewWidth(float currentWidth)
        {
            if (machineInfoLayout != null)
            {
                currentWidth = Mathf.Max(currentWidth, machineInfoLayout.preferredWidth + 2f * MachineInfoLayoutPadding);
            }

            if (byproductsLayout != null)
            {
                currentWidth = Mathf.Max(currentWidth, byproductsLayout.preferredWidth + 2f * ByproductLayoutPadding);
            }

            return currentWidth;
        }

        public static float IfExistsGetNewWidth(float currentWidth)
        {
            if (Instance == null)
            {
                return currentWidth;
            }

            return Instance.GetNewWidth(currentWidth);
        }

        public float PositionLayoutAndGetNewHeight(float width, float currentHeight)
        {
            if (byproductsLayout != null)
            {
                float preferredHeight = byproductsLayout.preferredHeight;
                byproductsLayout.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
                byproductsLayout.rectTransform.anchoredPosition = new Vector2(20f + width / 2f, -currentHeight - preferredHeight / 2f);

                currentHeight += preferredHeight;
            }

            if (machineInfoLayout != null)
            {
                float preferredHeight = machineInfoLayout.preferredHeight;
                machineInfoLayout.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, preferredHeight);
                machineInfoLayout.rectTransform.anchoredPosition = new Vector2(20f + width / 2f, -currentHeight - preferredHeight / 2f);

                currentHeight += preferredHeight;
            }

            return currentHeight;
        }

        public static float IfExistsPositionLayoutAndGetNewHeight(float width, float currentHeight)
        {
            if (Instance == null)
            {
                return currentHeight;
            }

            return Instance.PositionLayoutAndGetNewHeight(width, currentHeight);
        }

        public void UpdatePosition()
        {
            UpdatePositionMachine();
            UpdatePositionCraftTime();
            UpdatePositionIcons();
            UpdatePositionByproducts();
        }

        private void UpdatePositionMachine()
        {
            if (machine == null
                || !machine.isActiveAndEnabled)
            {
                return;
            }

            machine.title.SetScaleDirty();
        }

        private void UpdatePositionCraftTime()
        {
            if (craftTimeDisplay == null
                || !craftTimeDisplay.isActiveAndEnabled)
            {
                return;
            }

            craftTimeDisplay.title.SetScaleDirty();
        }

        private void UpdatePositionIcons()
        {
            if (extraIcons == null
                || extraIcons.Count == 0)
            {
                return;
            }

            foreach (uGUI_TooltipIcon icon in extraIcons)
            {
                if (!icon.isActiveAndEnabled)
                {
                    continue;
                }

                icon.title.SetScaleDirty();
            }
        }

        private void UpdatePositionByproducts()
        {
            if (byproductIcons == null
                || byproductIcons.Count == 0)
            {
                return;
            }

            foreach (uGUI_TooltipIcon icon in byproductIcons)
            {
                if (!icon.isActiveAndEnabled)
                {
                    continue;
                }

                icon.title.SetScaleDirty();
            }
        }

        public const float IconSize = 110f;
        public const float ByproductIconSize = 86f;
        public const float MachineInfoLayoutPadding = 10f;
        public const float ByproductLayoutPadding = 10f;
    }
}
