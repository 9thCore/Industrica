using Industrica.ClassBase.Addons.Machine;
using Industrica.ClassBase.Modules.ProcessingMachine;
using Industrica.Recipe.Handler;
using Industrica.Save;
using Industrica.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Industrica.ClassBase
{
    public abstract class BaseProcessingMachine : BaseMachine, IRelayPowerChangeListener
    {
        public GenericHandTarget handTarget;

        public GameObject itemModuleStorage;
        public List<ProcessingMachineItemModule> itemModules = new();
        public List<ProcessingMachineModule> modules = new();

        protected abstract bool CheckRecipe();
        protected abstract bool WorthToLookupRecipe();
        public abstract void PowerDownEvent(PowerRelay relay);

        public BaseProcessingMachine SetHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public M AddProcessingItemModule<M>() where M : ProcessingMachineItemModule, new()
        {
            if (itemModuleStorage == null)
            {
                itemModuleStorage = gameObject.CreateChild(nameof(itemModuleStorage));
            }

            M itemModule = itemModuleStorage.AddComponent<M>();
            itemModule.SetHolder(this);
            itemModules.Add(itemModule);
            modules.Add(itemModule);
            return itemModule;
        }

        public void QueueRecipeCheck()
        {
            StartCoroutine(DelayedRecipeCheck());
        }

        private IEnumerator DelayedRecipeCheck()
        {
            yield return null;
            CheckRecipe();
        }

        public void PowerUpEvent(PowerRelay relay)
        {
            while (CheckRecipe()) { }
        }

        private void UseModules<T>(List<T> modules) where T : ProcessingMachineModule
        {
            if (modules.Count == 0)
            {
                return;
            }

            const int firstElement = 0;
            modules[firstElement].Use(append: false, index: firstElement);

            for (int i = 1; i < modules.Count; i++)
            {
                modules[i].Use(append: true, index: i);
            }
        }

        protected virtual void OnHandClick(HandTargetEventData data)
        {
            UseModules(itemModules);

            Player.main.GetPDA().Open(PDATab.Inventory, transform);
        }

        protected virtual void OnHandHover(HandTargetEventData data)
        {
            HandReticle.main.SetText(HandReticle.TextType.Hand, "OpenStorage", true, GameInput.Button.LeftHand);
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);

            if (modules.All(module => module.IsEmpty()))
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Empty", true);
            }
        }
    }

    public abstract class BaseProcessingMachine<I, O, R, P, S> : BaseProcessingMachine
        where I : RecipeHandler.RecipeInput
        where O : RecipeHandler.RecipeOutput
        where R : RecipeHandler.Recipe<I, O>
        where P : BaseProcessingMachine<I, O, R, P, S>.Process, new()
        where S : BaseProcessingMachine<I, O, R, P, S>.SerialisedProcess, new()
    {
        protected I recipeInput;
        protected P currentProcess;

        public abstract void OnUpdate();

        public abstract bool CanProcess();
        public abstract void OnProgressProcess();
        public abstract float GetProcessingSpeed();
        public abstract bool TryFinishProcessing();
        public abstract void OnFinishRecipe();

        public abstract I GetRecipeInput();
        public abstract IEnumerable<R> GetRecipeStorage();
        public abstract bool TrySetupProcess(R recipe);

        public override void Start()
        {
            base.Start();

            foreach (ProcessingMachineModule module in itemModules)
            {
                module.OnStart();
            }

            handTarget.onHandClick = new();
            handTarget.onHandClick.AddListener(OnHandClick);
            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);

            recipeInput = GetRecipeInput();
        }

        public void Update()
        {
            OnUpdate();

            if (currentProcess == null
                || !currentProcess.readyToProcess
                || !CanProcess())
            {
                return;
            }

            if (currentProcess.timeRemaining > 0f)
            {
                currentProcess.timeRemaining -= DayNightCycle.main.deltaTime * GetProcessingSpeed();
                OnProgressProcess();
            } else
            {
                FinishProcessing();
            }
        }

        protected override bool CheckRecipe()
        {
            if (!WorthToLookupRecipe()
                || !RecipeHandler.TryGetRecipe<I, O, R>(GetRecipeStorage(), recipeInput, out R recipe)
                || !TrySetupProcess(recipe))
            {
                return false;
            }

            return true;
        }

        private void FinishProcessing()
        {
            if (currentProcess.timeRemaining > 0f
                || !TryFinishProcessing())
            {
                return;
            }

            currentProcess = null;
            OnFinishRecipe();
            CheckRecipe();
        }

        private void LoadCurrentProcess(S serialisedProcess)
        {
            if (serialisedProcess == null)
            {
                return;
            }

            currentProcess = serialisedProcess.Deserialise();
            OnProgressProcess();
        }

        public abstract class Process
        {
            public TechType input, output;
            public bool readyToProcess = true;
            public float timeRemaining;
            public float timeTotal;

            public virtual S Serialise()
            {
                return new()
                {
                    input = input,
                    output = output,
                    timeRemaining = timeRemaining,
                    timeTotal = timeTotal,
                };
            }
        }

        public abstract class SerialisedProcess
        {
            public TechType input, output;
            public float timeRemaining;
            public float timeTotal;

            public virtual P Deserialise()
            {
                return new()
                {
                    input = input,
                    output = output,
                    timeRemaining = timeRemaining,
                    timeTotal = timeTotal
                };
            }
        }

        public abstract class ProcessingSaveData<D, C> : ComponentSaveData<D, C>
            where D : ProcessingSaveData<D, C>
            where C : BaseProcessingMachine<I, O, R, P, S>
        {
            public S currentProcess;

            public ProcessingSaveData(C component) : base(component) { }

            public override void CopyFromStorage(D data)
            {
                currentProcess = data.currentProcess;
            }

            public override void Load()
            {
                Component.LoadCurrentProcess(currentProcess);
            }

            public override void Save()
            {
                if (Component.currentProcess == null)
                {
                    currentProcess = null;
                    return;
                }

                currentProcess = Component.currentProcess.Serialise();
            }

            public bool ShouldSerializecurrentProcess()
            {
                return currentProcess != null;
            }
        }
    }
}
