using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Utility
{
    public class GUIRecipeInformation : MonoBehaviour
    {
        public static GUIRecipeInformation Instance { get; private set; }

        private RecipeDisplayUtil.Information previousInformation;
        private RectTransform rectTransform;
        private FlexibleGridLayout machineInfoLayout;
        private uGUI_TooltipIcon machine;
        private uGUI_TooltipIcon craftTimeDisplay;
        private List<uGUI_TooltipIcon> extraIcons;
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
            }
        }
        
        private Transform GetOrCreateMachineInfoLayout()
        {
            if (machineInfoLayout == null)
            {
                GameObject layout = uGUI_Tooltip.main.iconCanvas.transform.parent.gameObject.CreateChild($"Industrica{nameof(machineInfoLayout)}");
                machineInfoLayout = layout.AddComponent<FlexibleGridLayout>();
                machineInfoLayout.horizontalAlignment = FlexibleGridLayout.HorizontalAlignment.Center;
                machineInfoLayout.rectTransform.anchorMin = Vector2.up;
                machineInfoLayout.rectTransform.anchorMax = Vector2.up;
                machineInfoLayout.clampMaxWidth = true;
                machineInfoLayout.useOnlyMinWidth = true;
            }

            return machineInfoLayout.transform;
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
            if (extraIcons == null)
            {
                extraIcons = new();
            }

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

        public void OnLayout()
        {
            float height = rectTransform.sizeDelta.y;
            OnLayoutMachine(ref height);

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        private void OnLayoutMachine(ref float height)
        {
            if (machineInfoLayout == null)
            {
                return;
            }

            machine.IfActiveCalculateLayoutInputHorizontal();
            craftTimeDisplay.IfActiveCalculateLayoutInputHorizontal();
            foreach (uGUI_TooltipIcon icon in extraIcons)
            {
                icon.IfActiveCalculateLayoutInputHorizontal();
            }
            machineInfoLayout.CalculateLayoutInputHorizontal();

            machineInfoLayout.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rectTransform.sizeDelta.x);

            machineInfoLayout.SetLayoutHorizontal();
            machine.IfActiveSetLayoutHorizontal();
            craftTimeDisplay.IfActiveSetLayoutHorizontal();
            foreach (uGUI_TooltipIcon icon in extraIcons)
            {
                icon.IfActiveSetLayoutHorizontal();
            }

            machine.IfActiveCalculateLayoutInputVertical();
            craftTimeDisplay.IfActiveCalculateLayoutInputVertical();
            foreach (uGUI_TooltipIcon icon in extraIcons)
            {
                icon.IfActiveCalculateLayoutInputVertical();
            }
            machineInfoLayout.CalculateLayoutInputVertical();

            machineInfoLayout.SetLayoutVertical();
            machine.IfActiveSetLayoutVertical();
            craftTimeDisplay.IfActiveSetLayoutVertical();
            foreach (uGUI_TooltipIcon icon in extraIcons)
            {
                icon.IfActiveSetLayoutVertical();
            }

            machineInfoLayout.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, machineInfoLayout.preferredHeight);
            machineInfoLayout.rectTransform.anchoredPosition = new Vector2(rectTransform.sizeDelta.x / 2f, rectTransform.offsetMin.y - machineInfoLayout.preferredHeight / 2f);

            height += machineInfoLayout.rectTransform.sizeDelta.y;
        }

        public void UpdatePosition()
        {
            UpdatePositionMachine();
            UpdatePositionCraftTime();
            UpdatePositionIcons();
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

        public const float IconSize = 110f;
    }
}
