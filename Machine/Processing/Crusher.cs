using Industrica.Buildable.Processing;
using Industrica.ClassBase;
using Industrica.ClassBase.Modules.ProcessingMachine;
using Industrica.Recipe.Handler;
using Industrica.Save;
using Industrica.Utility;
using Industrica.Utility.Smooth;
using Nautilus.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace Industrica.Machine.Processing
{
    public class Crusher : BaseProcessingMachine<
        CrusherRecipeHandler.Recipe.Input,
        CrusherRecipeHandler.Recipe.Output,
        CrusherRecipeHandler.Recipe,
        Crusher.CrusherProcess,
        Crusher.CrusherSerialisedProcess>
    {
        public ProcessingMachineItemModule inputModule, temporaryOutputModule, outputModule;

        private Sprite inputSprite, outputSprite;
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

        public override bool CanProcess()
        {
            return IsPowered();
        }

        public override float GetProcessingSpeed()
        {
            return 1f;
        }

        public override CrusherRecipeHandler.Recipe.Input GetRecipeInput()
        {
            return new(inputModule.ItemsContainer);
        }

        public override IEnumerable<CrusherRecipeHandler.Recipe> GetRecipeStorage()
        {
            return CrusherRecipeHandler.Recipes;
        }

        public override void OnUpdate()
        {
            if (currentProcess == null)
            {
                return;
            }

            ConsumeEnergyPerSecond(EnergyUsage, out _);
        }

        public override void OnProgressProcess()
        {
            UpdateProcessUI();
        }

        public override void PowerDownEvent(PowerRelay relay)
        {

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

        public override void OnFinishRecipe()
        {
            inputSprite = null;
            outputSprite = null;
            UpdateProcessUI();
        }

        public override bool TrySetupProcess(CrusherRecipeHandler.Recipe recipe)
        {
            List<Pickupable> items = recipe.GetUsedItems(recipeInput).ToList();
            CrusherRecipeHandler.Recipe.Output[] outputs = recipe.Outputs;

            if (!temporaryOutputModule.ItemsContainer.HasRoomFor(outputs))
            {
                return false;
            }

            float craftTime = recipe.Data.CraftTime;

            StartCoroutine(StartCrushing(
                outputs,
                items,
                craftTime));

            return true;
        }

        protected override bool WorthToLookupRecipe()
        {
            return !inputModule.ItemsContainer.IsEmpty();
        }

        protected override void OnHandClick(HandTargetEventData data)
        {
            base.OnHandClick(data);

            BuildableCrusher.ProcessingItem.Show();

            BuildableCrusher.ProcessProgressBarHint.Show();
            BuildableCrusher.ProcessProgressBar.Show();
            BuildableCrusher.ProcessPercentage.Show();

            BuildableCrusher.LeftDecorativeGear.Show();
            BuildableCrusher.RightDecorativeGear.Show();

            BuildableCrusher.LeftGear.Show();
            BuildableCrusher.RightGear.Show();

            BuildableCrusher.ProcessingItem.Rotation = UnityEngine.Random.Range(-BuildableCrusher.CrushRotationRange, BuildableCrusher.CrushRotationRange);

            UpdateProcessUI();
        }

        private void UpdateProcessUI()
        {
            if (!IsStorageOpen())
            {
                return;
            }

            if (currentProcess == null)
            {
                BuildableCrusher.ProcessingItem.SetActive(false);
                BuildableCrusher.ProcessPercentage.Percentage = 0;
                BuildableCrusher.ProcessProgressBar.Percentage = 0f;
                return;
            }

            float elapsed = currentProcess.timeTotal - currentProcess.timeRemaining;
            float normalisedPercentage = elapsed / currentProcess.timeTotal;
            BuildableCrusher.ProcessPercentage.Percentage = (int)(100f * normalisedPercentage);
            BuildableCrusher.ProcessProgressBar.Percentage = normalisedPercentage;

            BuildableCrusher.ProcessingItem.SetActive(true);

            if (inputSprite == null)
            {
                inputSprite = SpriteManager.Get(currentProcess.input);
            }

            if (outputSprite == null)
            {
                outputSprite = SpriteManager.Get(currentProcess.output);
            }

            Vector2 scale;

            if (currentProcess.timeRemaining > currentProcess.timeTotal / 2f)
            {
                BuildableCrusher.ProcessingItem.Sprite = inputSprite;
                scale = math.lerp(Vector2.one, Vector2.zero, normalisedPercentage * 2f);
            }
            else
            {
                BuildableCrusher.ProcessingItem.Sprite = outputSprite;
                scale = math.lerp(Vector2.zero, Vector2.one, (normalisedPercentage - 0.5f) * 2f);
            }

            Vector2 position = math.lerp(BuildableCrusher.ItemInputUIPosition, BuildableCrusher.ItemOutputUIPosition, normalisedPercentage);
            BuildableCrusher.ProcessingItem.Position = position;

            BuildableCrusher.ProcessingItem.Scale = scale;
        }

        private IEnumerator StartCrushing(
            CrusherRecipeHandler.Recipe.Output[] outputs,
            List<Pickupable> items,
            float craftTime)
        {
            currentProcess = new()
            {
                input = items[0].GetTechType(),
                output = outputs[0].TechType,
                timeRemaining = craftTime,
                timeTotal = craftTime,
                readyToProcess = false
            };

            foreach (Pickupable item in items)
            {
                item.inventoryItem.container.RemoveItem(item.inventoryItem, true, false);
                GameObject.Destroy(item.gameObject);
            }

            foreach (CrusherRecipeHandler.Recipe.Output output in outputs)
            {
                yield return output.RunOnInventoryItems(temporaryOutputModule.ItemsContainer.UnsafeAdd);
            }

            currentProcess.readyToProcess = true;
        }

        public override void Start()
        {
            base.Start();

            saveData = new(this);
        }

        public void OnDestroy()
        {
            saveData.Invalidate();
        }

        public const float EnergyUsage = 1.5f;

        public class CrusherProcess : Process { }
        public class CrusherSerialisedProcess : SerialisedProcess { }

        public class SaveData : ProcessingSaveData<SaveData, Crusher>
        {
            public SaveData(Crusher component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.crusherSaveData;
        }
    }
}
