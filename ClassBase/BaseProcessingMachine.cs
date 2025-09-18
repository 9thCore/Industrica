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

    public abstract class BaseProcessingMachine<I, O, R, G, S> : BaseProcessingMachine
        where I : RecipeHandler.RecipeInput
        where O : RecipeHandler.RecipeOutput
        where R : RecipeHandler.Recipe<I, O>
        where G : BaseProcessingMachine<I, O, R, G, S>.Group
        where S : BaseProcessingMachine<I, O, R, G, S>.SerialisedGroup
    {
        protected I recipeInput;
        protected readonly List<G> groups = new();

        public abstract void OnUpdate();

        public abstract bool CanProcessGroups();
        public abstract bool CanProcess(G group);
        public abstract float GetProcessingSpeed(G group);
        public abstract bool TryFinishProcessing(G group);

        public abstract I GetRecipeInput();
        public abstract IEnumerable<R> GetRecipeStorage();
        public abstract bool TryStartGroupCreation(R recipe);

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

            if (groups.Count == 0
                || !CanProcessGroups())
            {
                return;
            }

            for (int i = groups.Count - 1; i >= 0; i--)
            {
                G group = groups[i];

                if (!group.readyToProcess
                    || !CanProcess(group))
                {
                    continue;
                }

                if (group.timeRemaining > 0f)
                {
                    group.timeRemaining -= DayNightCycle.main.deltaTime * GetProcessingSpeed(group);
                }

                FinishProcessing(group, i);
            }
        }

        protected override bool CheckRecipe()
        {
            if (!WorthToLookupRecipe()
                || !RecipeHandler.TryGetRecipe<I, O, R>(GetRecipeStorage(), recipeInput, out R recipe)
                || !TryStartGroupCreation(recipe))
            {
                return false;
            }

            return true;
        }

        private void FinishProcessing(G group, int index)
        {
            if (group.timeRemaining > 0f
                || !TryFinishProcessing(group))
            {
                return;
            }

            groups.RemoveAt(index);
            while (CheckRecipe()) { }
        }

        private void LoadGroups(IEnumerable<S> serializedGroups)
        {
            if (serializedGroups == null)
            {
                return;
            }

            foreach (S serializedGroup in serializedGroups)
            {
                if (serializedGroup.TryDeserialise(out G group))
                {
                    groups.Add(group);
                }
            }
        }

        public abstract class Group
        {
            public bool readyToProcess = true;
            public float timeRemaining;
            public float timeTotal;

            public abstract bool TrySerialise(out S serialisedGroup);
        }

        public abstract class SerialisedGroup
        {
            public float timeRemaining;
            public float timeTotal;

            public abstract bool TryDeserialise(out G group);
        }

        public abstract class ProcessingSaveData<D, C> : ComponentSaveData<D, C>
            where D : ProcessingSaveData<D, C>
            where C : BaseProcessingMachine<I, O, R, G, S>
        {
            public List<S> groups;

            public ProcessingSaveData(C component) : base(component) { }

            public override void CopyFromStorage(D data)
            {
                groups = data.groups;
            }

            public override void Load()
            {
                Component.LoadGroups(groups);
            }

            public override void Save()
            {
                if (Component.groups.Count == 0)
                {
                    groups = null;
                    return;
                }

                groups = new();

                foreach (G group in Component.groups)
                {
                    if (group.TrySerialise(out S serialisedGroup))
                    {
                        groups.Add(serialisedGroup);
                    }
                }
            }

            public bool ShouldSerializegroups()
            {
                return groups != null
                    && groups.Count > 0;
            }
        }
    }
}
