using Industrica.Buildable.Processing;
using Industrica.ClassBase;
using Industrica.ClassBase.Addons.Machine;
using Industrica.Recipe.Handler;
using Industrica.Save;
using Industrica.UI.Bar;
using Industrica.UI.UIData;
using Industrica.Utility;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.Machine.Processing
{
    public class Smeltery : BaseMachine, IRelayPowerChangeListener
    {
        public GenericHandTarget handTarget;
        public StorageContainer input;
        public StorageContainer output;
        public StorageContainer chamber;
        public StorageContainer preOutput;

        public TextUIData heatInfoUIData;
        public BackedTextUIData noHeatUIData, lowHeatUIData, medHeatUIData, highHeatUIData;

        private SmelteryRecipeHandler.HeatLevel currentHeat = SmelteryRecipeHandler.HeatLevel.None;
        private SmelteryRecipeHandler.Recipe cachedRecipe;
        private readonly List<SmelteryGroup> groups = new();
        private SaveData saveData;

        public Smeltery WithHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public Smeltery WithInput(StorageContainer input)
        {
            this.input = input;
            return this;
        }

        public Smeltery WithOutput(StorageContainer output)
        {
            this.output = output;
            return this;
        }

        public Smeltery WithChamber(StorageContainer chamber)
        {
            this.chamber = chamber;
            return this;
        }

        public Smeltery WithPreOutput(StorageContainer preOutput)
        {
            this.preOutput = preOutput;
            return this;
        }

        public Smeltery SetupUI(GameObject screen)
        {
            noHeatUIData = screen.CreateChild(nameof(noHeatUIData)).EnsureComponent<BackedTextUIData>();
            lowHeatUIData = screen.CreateChild(nameof(lowHeatUIData)).EnsureComponent<BackedTextUIData>();
            medHeatUIData = screen.CreateChild(nameof(medHeatUIData)).EnsureComponent<BackedTextUIData>();
            highHeatUIData = screen.CreateChild(nameof(highHeatUIData)).EnsureComponent<BackedTextUIData>();
            heatInfoUIData = screen.CreateChild(nameof(heatInfoUIData)).EnsureComponent<TextUIData>();
            return this;
        }

        public void SetNoHeat()
        {
            currentHeat = SmelteryRecipeHandler.HeatLevel.None;
            UpdateUI();
        }

        public void SetLowHeat()
        {
            currentHeat = SmelteryRecipeHandler.HeatLevel.Low;
            UpdateUI();
        }

        public void SetMediumHeat()
        {
            currentHeat = SmelteryRecipeHandler.HeatLevel.Medium;
            UpdateUI();
        }

        public void SetHighHeat()
        {
            currentHeat = SmelteryRecipeHandler.HeatLevel.High;
            UpdateUI();
        }

        private void UpdateUI()
        {
            UpdateUIFor(noHeatUIData, currentHeat == SmelteryRecipeHandler.HeatLevel.None);
            UpdateUIFor(lowHeatUIData, currentHeat == SmelteryRecipeHandler.HeatLevel.Low);
            UpdateUIFor(medHeatUIData, currentHeat == SmelteryRecipeHandler.HeatLevel.Medium);
            UpdateUIFor(highHeatUIData, currentHeat == SmelteryRecipeHandler.HeatLevel.High);
        }

        private void UpdateUIFor(BackedTextUIData data, bool selected)
        {
            data.background.sprite = SpriteManager.GetBackground(selected ? BuildableSmeltery.ActiveType : BuildableSmeltery.InactiveType);
        }

        public override void Start()
        {
            base.Start();

            input.isValidHandTarget = false;
            output.isValidHandTarget = false;
            chamber.isValidHandTarget = false;
            preOutput.isValidHandTarget = false;

            handTarget.onHandClick = new();
            handTarget.onHandClick.AddListener(OnHandClick);
            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);

            input.container.onAddItem += OnAddItem;
            output.container.onRemoveItem += OnRemoveItem;
            
            output.container.isAllowedToAdd = SNUtil.DisallowAction;
            chamber.container.isAllowedToAdd = SNUtil.DisallowAction;
            chamber.container.isAllowedToRemove = SNUtil.DisallowAction;
            preOutput.container.isAllowedToAdd = SNUtil.DisallowAction;
            preOutput.container.isAllowedToRemove = SNUtil.DisallowAction;

            noHeatUIData.AddOnClickCallback(SetNoHeat);
            lowHeatUIData.AddOnClickCallback(SetLowHeat);
            medHeatUIData.AddOnClickCallback(SetMediumHeat);
            highHeatUIData.AddOnClickCallback(SetHighHeat);

            saveData = new(this);
        }

        public void Update()
        {
            switch (currentHeat)
            {
                case SmelteryRecipeHandler.HeatLevel.Low:
                    ConsumeEnergyPerSecond(LowHeatEnergyUsage, out _);
                    break;
                case SmelteryRecipeHandler.HeatLevel.Medium:
                    ConsumeEnergyPerSecond(MediumHeatEnergyUsage, out _);
                    break;
                case SmelteryRecipeHandler.HeatLevel.High:
                    ConsumeEnergyPerSecond(HighHeatEnergyUsage, out _);
                    break;
                default:
                    break;
            }

            if (groups.Count == 0
                || !IsPowered())
            {
                return;
            }

            for (int i = groups.Count - 1; i >= 0; i--)
            {
                SmelteryGroup group = groups[i];

                if (group.requiredHeatLevel > currentHeat
                    || group.timeRemaining <= 0f)
                {
                    continue;
                }

                group.timeRemaining -= DayNightCycle.main.deltaTime * SmelteryRecipeHandler.GetSpeedMultiplier(currentHeat, group.requiredHeatLevel);
                FinishSmelting(group, i);
            }
        }

        private void FinishSmelting(SmelteryGroup group, int index)
        {
            if (group.timeRemaining > 0f
                || !output.container.HasRoomFor(group.outputs))
            {
                return;
            }

            group.inputs.ForEach(pickupable =>
            {
                chamber.container.RemoveItem(pickupable, true);
                GameObject.Destroy(pickupable.gameObject);
            });

            group.outputs.ForEach(pickupable =>
            {
                preOutput.container.RemoveItem(pickupable, true);
                output.container.UnsafeAdd(new InventoryItem(pickupable));
            });

            groups.RemoveAt(index);

            while (MoveIntoChamberIfAvailable()) { }
        }

        public void OnDestroy()
        {
            saveData.Invalidate();
        }

        private void OnAddItem(InventoryItem item)
        {
            StartCoroutine(MoveIntoChamberIfAvailableLate());
        }

        private void OnRemoveItem(InventoryItem item)
        {
            for (int i = groups.Count - 1; i >= 0; i--)
            {
                SmelteryGroup group = groups[i];
                FinishSmelting(group, i);
            }
        }

        private IEnumerator MoveIntoChamberIfAvailableLate()
        {
            yield return null;
            MoveIntoChamberIfAvailable();
        }

        private bool MoveIntoChamberIfAvailable()
        {
            if (input.container.IsEmpty())
            {
                return false;
            }

            SmelteryRecipeHandler.Recipe.Input recipeInput = new(input.container.ToArray());

            if (cachedRecipe == null
                || !cachedRecipe.Test(recipeInput))
            {
                if (!RecipeHandler.TryGetRecipe<SmelteryRecipeHandler.Recipe.Input, SmelteryRecipeHandler.Recipe.Output, SmelteryRecipeHandler.Recipe>(
                    SmelteryRecipeHandler.Recipes,
                    recipeInput,
                    out SmelteryRecipeHandler.Recipe recipe))
                {
                    return false;
                }

                cachedRecipe = recipe;
            }

            IEnumerable<Pickupable> items = cachedRecipe.GetUsedItems(recipeInput);
            SmelteryRecipeHandler.Recipe.Output[] outputs = cachedRecipe.Outputs;

            if (!chamber.container.HasRoomFor(items)
                || !preOutput.container.HasRoomFor(outputs))
            {
                return false;
            }

            StartCoroutine(StartSmelting(
                outputs,
                items,
                cachedRecipe.RequiredHeatLevel,
                cachedRecipe.CraftTime));

            return true;
        }

        private IEnumerator StartSmelting(
            SmelteryRecipeHandler.Recipe.Output[] outputs,
            IEnumerable<Pickupable> items,
            SmelteryRecipeHandler.HeatLevel requiredHeatLevel,
            float craftTime)
        {
            SmelteryGroup group = new()
            {
                timeRemaining = craftTime,
                timeTotal = craftTime,
                requiredHeatLevel = requiredHeatLevel
            };
            groups.Add(group);

            group.inputs.AddRange(items);

            foreach (Pickupable item in items)
            {
                item.inventoryItem.isEnabled = false;
                item.inventoryItem.container.RemoveItem(item.inventoryItem, true, false);
                chamber.container.UnsafeAdd(item.inventoryItem);
            }

            group.PrepareInputs();

            foreach (SmelteryRecipeHandler.Recipe.Output output in outputs)
            {
                yield return output.RunOnItems(pickupable =>
                {
                    group.outputs.Add(pickupable);
                    preOutput.container.UnsafeAdd(new InventoryItem(pickupable));
                });
            }
        }
        
        public void OnHandClick(HandTargetEventData data)
        {
            Inventory.main.SetUsedStorage(input.container);
            Inventory.main.SetUsedStorage(chamber.container, true);
            Inventory.main.SetUsedStorage(output.container, true);

            UICustomContainerHandler.MoveContainerUI(UICustomContainerHandler.Torpedo1, BuildableSmeltery.InputContainerUIPosition);
            UICustomContainerHandler.MoveContainerUI(UICustomContainerHandler.Torpedo2, BuildableSmeltery.ChamberContainerUIPosition);
            UICustomContainerHandler.MoveContainerUI(UICustomContainerHandler.Torpedo3, BuildableSmeltery.OutputContainerUIPosition);

            Player.main.GetPDA().Open(PDATab.Inventory, transform);
        }

        public void OnHandHover(HandTargetEventData data)
        {
            HandReticle.main.SetText(HandReticle.TextType.Hand, "OpenStorage", true, GameInput.Button.LeftHand);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);

            if (input.IsEmpty()
                && output.IsEmpty()
                && chamber.IsEmpty())
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Empty", true);
            }
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            while (MoveIntoChamberIfAvailable()) { }
        }

        public void PowerDownEvent(PowerRelay relay)
        {
            currentHeat = SmelteryRecipeHandler.HeatLevel.None;
            UpdateUI();
        }

        private void LoadGroups(IEnumerable<SaveData.SerializedSmelteryGroup> serializedGroups)
        {
            if (serializedGroups == null)
            {
                return;
            }

            foreach (SaveData.SerializedSmelteryGroup serializedGroup in serializedGroups)
            {
                if (serializedGroup.inputs == null
                    || serializedGroup.outputs == null)
                {
                    Plugin.Logger.LogWarning("Skipping group that does not have inputs or outputs");
                    continue;
                }

                SmelteryGroup group = new()
                {
                    timeRemaining = serializedGroup.timeRemaining,
                    timeTotal = serializedGroup.timeTotal,
                    requiredHeatLevel = serializedGroup.requiredHeatLevel
                };

                SNUtil.RestoreItems(serializedGroup.inputs, group.inputs);
                SNUtil.RestoreItems(serializedGroup.outputs, group.outputs);

                group.PrepareInputs();
                groups.Add(group);
            }
        }

        public const float LowHeatEnergyUsage = 0.5f;
        public const float MediumHeatEnergyUsage = 1f;
        public const float HighHeatEnergyUsage = 2.5f;

        public class SmelteryGroup
        {
            public readonly List<Pickupable> inputs = new();
            public readonly List<Pickupable> outputs = new();
            public float timeRemaining;
            public float timeTotal;
            public SmelteryRecipeHandler.HeatLevel requiredHeatLevel;

            public void PrepareInputs()
            {
                inputs.ForEach(pickupable =>
                {
                    pickupable.gameObject.EnsureComponent<SmelteryItemBar>().group = this;
                });
            }
        }

        public class SaveData : ComponentSaveData<SaveData, Smeltery>
        {
            public SmelteryRecipeHandler.HeatLevel currentHeat;
            public List<SerializedSmelteryGroup> groups;

            public SaveData(Smeltery component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.smelterySaveData;

            public override void CopyFromStorage(SaveData data)
            {
                currentHeat = data.currentHeat;
                groups = data.groups;
            }

            public override void Load()
            {
                Component.currentHeat = currentHeat;
                Component.LoadGroups(groups);
                Component.UpdateUI();
            }

            public override void Save()
            {
                currentHeat = Component.currentHeat;

                if (Component.groups.Count == 0)
                {
                    groups = null;
                } else
                {
                    groups = new();
                    foreach (SmelteryGroup group in Component.groups)
                    {
                        groups.Add(new()
                        {
                            inputs = SNUtil.SerializeReferences(group.inputs),
                            outputs = SNUtil.SerializeReferences(group.outputs),
                            timeRemaining = group.timeRemaining,
                            timeTotal = group.timeTotal,
                            requiredHeatLevel = group.requiredHeatLevel,
                        });
                    }
                }
            }

            public bool ShouldSerializegroups()
            {
                return groups != null
                    && groups.Count > 0;
            }

            public class SerializedSmelteryGroup
            {
                public List<string> inputs;
                public List<string> outputs;
                public float timeRemaining;
                public float timeTotal;
                public SmelteryRecipeHandler.HeatLevel requiredHeatLevel;
            }
        }
    }
}
