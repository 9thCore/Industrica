using Industrica.Buildable.Processing;
using Industrica.ClassBase;
using Industrica.ClassBase.Modules.ProcessingMachine;
using Industrica.Recipe.Handler;
using Industrica.Save;
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
        Smeltery.SmelteryProcess,
        Smeltery.SerialisedSmelteryProcess>
    {
        public TextUIData heatInfoUIData;
        public BackedTextUIData noHeatUIData, lowHeatUIData, medHeatUIData, highHeatUIData;
        public ProcessingMachineItemModule inputModule, temporaryOutputModule, outputModule;

        private SmelteryRecipeHandler.HeatLevel currentHeat = SmelteryRecipeHandler.HeatLevel.None;
        private SaveData saveData;

        public ProcessingMachineItemModule BeginInputModuleSetup()
        {
            inputModule = AddProcessingItemModule<ProcessingMachineItemModule>();
            return inputModule;
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

        public override float GetProcessingSpeed()
        {
            return SmelteryRecipeHandler.GetSpeedMultiplier(currentHeat, currentProcess.requiredHeatLevel);
        }

        public override bool CanProcess()
        {
            return currentProcess.requiredHeatLevel <= currentHeat
                && IsPowered();
        }

        public override bool TryFinishProcessing()
        {
            if (!outputModule.ItemsContainer.HasRoomFor(temporaryOutputModule.ItemsContainer))
            {
                return false;
            }

            List<InventoryItem> outputs = new(temporaryOutputModule.ItemsContainer);
            outputs.ForEach(inventoryItem =>
            {
                Pickupable pickupable = inventoryItem.item;
                temporaryOutputModule.ItemsContainer.RemoveItem(pickupable, true);
                outputModule.ItemsContainer.UnsafeAdd(new InventoryItem(pickupable));
            });

            return true;
        }

        public override SmelteryRecipeHandler.Recipe.Input GetRecipeInput()
        {
            return new(inputModule.ItemsContainer);
        }

        public override bool TrySetupProcess(SmelteryRecipeHandler.Recipe recipe)
        {
            List<Pickupable> items = recipe.GetUsedItems(recipeInput).ToList();
            SmelteryRecipeHandler.Recipe.Output[] outputs = recipe.Outputs;

            if (!temporaryOutputModule.ItemsContainer.HasRoomFor(outputs))
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
            currentProcess = new()
            {
                timeRemaining = craftTime,
                timeTotal = craftTime,
                requiredHeatLevel = requiredHeatLevel,
                readyToProcess = false
            };

            foreach (Pickupable item in items)
            {
                item.inventoryItem.container.RemoveItem(item.inventoryItem, true, false);
                GameObject.Destroy(item.gameObject);
            }

            foreach (SmelteryRecipeHandler.Recipe.Output output in outputs)
            {
                yield return output.RunOnInventoryItems(temporaryOutputModule.ItemsContainer.UnsafeAdd);
            }

            currentProcess.readyToProcess = true;
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

        public class SmelteryProcess : Process
        {
            public SmelteryRecipeHandler.HeatLevel requiredHeatLevel;

            public override SerialisedSmelteryProcess Serialise()
            {
                SerialisedSmelteryProcess serialisedProcess = base.Serialise();
                serialisedProcess.requiredHeatLevel = requiredHeatLevel;
                return serialisedProcess;
            }
        }

        public class SerialisedSmelteryProcess : SerialisedProcess
        {
            public SmelteryRecipeHandler.HeatLevel requiredHeatLevel;

            public override SmelteryProcess Deserialise()
            {
                SmelteryProcess process = base.Deserialise();
                process.requiredHeatLevel = requiredHeatLevel;
                return process;
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
