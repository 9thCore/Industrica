using Industrica.Buildable.Processing;
using Industrica.ClassBase;
using Industrica.ClassBase.Modules.ProcessingMachine;
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
    public class Smeltery : BaseProcessingMachine<
        SmelteryRecipeHandler.Recipe.Input,
        SmelteryRecipeHandler.Recipe.Output,
        SmelteryRecipeHandler.Recipe,
        Smeltery.SmelteryGroup,
        Smeltery.SerialisedSmelteryGroup>
    {
        public TextUIData heatInfoUIData;
        public BackedTextUIData noHeatUIData, lowHeatUIData, medHeatUIData, highHeatUIData;
        public ProcessingMachineItemModule inputModule, chamberModule, temporaryOutputModule, outputModule;

        private SmelteryRecipeHandler.HeatLevel currentHeat = SmelteryRecipeHandler.HeatLevel.None;
        private SaveData saveData;

        public ProcessingMachineItemModule BeginInputModuleSetup()
        {
            inputModule = AddProcessingItemModule<ProcessingMachineItemModule>();
            return inputModule;
        }

        public ProcessingMachineItemModule BeginChamberModuleSetup()
        {
            chamberModule = AddProcessingItemModule<ProcessingMachineItemModule>();
            return chamberModule;
        }

        public ProcessingMachineItemModule BeginPreOutputModuleSetup()
        {
            temporaryOutputModule = AddProcessingItemModule<ProcessingMachineItemModule>();
            return temporaryOutputModule;
        }

        public ProcessingMachineItemModule BeginOutputModuleSetup()
        {
            outputModule = AddProcessingItemModule<ProcessingMachineItemModule>();
            return outputModule;
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

            noHeatUIData.AddOnClickCallback(SetNoHeat);
            lowHeatUIData.AddOnClickCallback(SetLowHeat);
            medHeatUIData.AddOnClickCallback(SetMediumHeat);
            highHeatUIData.AddOnClickCallback(SetHighHeat);

            saveData = new(this);
        }

        public override void OnUpdate()
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
        }
        
        public override bool CanProcessGroups() => IsPowered();

        public override float GetProcessingSpeed(SmelteryGroup group)
        {
            return SmelteryRecipeHandler.GetSpeedMultiplier(currentHeat, group.requiredHeatLevel);
        }

        public override bool CanProcess(SmelteryGroup group)
        {
            return group.requiredHeatLevel <= currentHeat;
        }

        public override bool TryFinishProcessing(SmelteryGroup group)
        {
            if (!outputModule.ItemsContainer.HasRoomFor(group.outputs))
            {
                return false;
            }

            group.inputs.ForEach(pickupable =>
            {
                chamberModule.ItemsContainer.RemoveItem(pickupable, true);
                GameObject.Destroy(pickupable.gameObject);
            });

            group.outputs.ForEach(pickupable =>
            {
                temporaryOutputModule.ItemsContainer.RemoveItem(pickupable, true);
                outputModule.ItemsContainer.UnsafeAdd(new InventoryItem(pickupable));
            });

            return true;
        }

        public override SmelteryRecipeHandler.Recipe.Input GetRecipeInput()
        {
            return new(inputModule.ItemsContainer);
        }

        public override bool TryStartGroupCreation(SmelteryRecipeHandler.Recipe recipe)
        {
            List<Pickupable> items = recipe.GetUsedItems(recipeInput).ToList();
            SmelteryRecipeHandler.Recipe.Output[] outputs = recipe.Outputs;

            if (!chamberModule.ItemsContainer.HasRoomFor(items)
                || !temporaryOutputModule.ItemsContainer.HasRoomFor(outputs))
            {
                return false;
            }

            StartCoroutine(StartSmelting(
                outputs,
                items,
                recipe.RequiredHeatLevel,
                recipe.Data.CraftTime));

            return true;
        }

        protected override bool WorthToLookupRecipe()
        {
            return !inputModule.ItemsContainer.IsEmpty();
        }

        public override IEnumerable<SmelteryRecipeHandler.Recipe> GetRecipeStorage()
        {
            return SmelteryRecipeHandler.Recipes;
        }

        private IEnumerator StartSmelting(
            SmelteryRecipeHandler.Recipe.Output[] outputs,
            List<Pickupable> items,
            SmelteryRecipeHandler.HeatLevel requiredHeatLevel,
            float craftTime)
        {
            SmelteryGroup group = new()
            {
                timeRemaining = craftTime,
                timeTotal = craftTime,
                requiredHeatLevel = requiredHeatLevel,
                readyToProcess = false
            };

            groups.Add(group);

            group.inputs.AddRange(items);

            foreach (Pickupable item in items)
            {
                item.inventoryItem.container.RemoveItem(item.inventoryItem, true, false);
                chamberModule.ItemsContainer.UnsafeAdd(item.inventoryItem);
            }

            group.PrepareInputs();

            foreach (SmelteryRecipeHandler.Recipe.Output output in outputs)
            {
                yield return output.RunOnItems(pickupable =>
                {
                    group.outputs.Add(pickupable);
                    temporaryOutputModule.ItemsContainer.UnsafeAdd(new InventoryItem(pickupable));
                });
            }

            group.readyToProcess = true;
        }

        public void OnDestroy()
        {
            saveData.Invalidate();
        }

        public override void PowerDownEvent(PowerRelay relay)
        {
            currentHeat = SmelteryRecipeHandler.HeatLevel.None;
            UpdateUI();
        }

        public const float LowHeatEnergyUsage = 0.5f;
        public const float MediumHeatEnergyUsage = 1f;
        public const float HighHeatEnergyUsage = 2.5f;

        public class SmelteryGroup : Group
        {
            public readonly List<Pickupable> inputs = new();
            public readonly List<Pickupable> outputs = new();
            public SmelteryRecipeHandler.HeatLevel requiredHeatLevel;

            public void PrepareInputs()
            {
                inputs.ForEach(pickupable =>
                {
                    pickupable.gameObject.EnsureComponent<SmelteryItemBar>().group = this;
                    pickupable.inventoryItem.isEnabled = false;
                });
            }

            public override bool TrySerialise(out SerialisedSmelteryGroup serialisedGroup)
            {
                if (inputs == null
                    || inputs.Count == 0
                    || outputs == null
                    || outputs.Count == 0)
                {
                    serialisedGroup = default;
                    return false;
                }

                serialisedGroup = new()
                {
                    inputs = SNUtil.SerializeReferences(inputs),
                    outputs = SNUtil.SerializeReferences(outputs),
                    requiredHeatLevel = requiredHeatLevel,
                    timeRemaining = timeRemaining,
                    timeTotal = timeTotal
                };

                return true;
            }
        }

        public class SerialisedSmelteryGroup : SerialisedGroup
        {
            public List<string> inputs;
            public List<string> outputs;
            public SmelteryRecipeHandler.HeatLevel requiredHeatLevel;

            public override bool TryDeserialise(out SmelteryGroup group)
            {
                if (inputs == null
                    || inputs.Count == 0
                    || outputs == null
                    || outputs.Count == 0)
                {
                    group = default;
                    return false;
                }

                group = new()
                {
                    requiredHeatLevel = requiredHeatLevel,
                    timeRemaining = timeRemaining,
                    timeTotal = timeTotal
                };

                SNUtil.RestoreItems(inputs, group.inputs);
                SNUtil.RestoreItems(outputs, group.outputs);

                group.PrepareInputs();
                return true;
            }
        }

        public class SaveData : ProcessingSaveData<SaveData, Smeltery>
        {
            public SmelteryRecipeHandler.HeatLevel currentHeat;

            public SaveData(Smeltery component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.smelterySaveData;

            public override void CopyFromStorage(SaveData data)
            {
                base.CopyFromStorage(data);
                currentHeat = data.currentHeat;
            }

            public override void Load()
            {
                base.Load();
                Component.currentHeat = currentHeat;
                Component.UpdateUI();
            }

            public override void Save()
            {
                base.Save();
                currentHeat = Component.currentHeat;
            }
        }
    }
}
