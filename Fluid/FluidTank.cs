using Industrica.Fluid.Generic;
using Industrica.Save;
using Industrica.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Fluid
{
    public class FluidTank : MonoBehaviour
    {
        internal readonly FluidStack fluidStack = new();
        public GenericHandTarget handTarget;
        public int maxAmount;
        
        private SaveData saveData;
        private readonly HashSet<ISubscriber> subscribers = new();

        public FluidTank SetHandTarget(GenericHandTarget handTarget)
        {
            this.handTarget = handTarget;
            return this;
        }

        public FluidTank WithMaxAmount(int maxAmount)
        {
            this.maxAmount = maxAmount;
            return this;
        }

        public void Start()
        {
            saveData = new(this);

            handTarget.onHandHover = new();
            handTarget.onHandHover.AddListener(OnHandHover);
        }

        protected virtual void OnHandHover(HandTargetEventData data)
        {
            if (Empty())
            {
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, "Empty", true);
            } else
            {
                string format = "IndustricaFluidStackFormat".Translate(fluidStack.amount, maxAmount, fluidStack.techType.AsString().Translate());
                HandReticle.main.SetText(HandReticle.TextType.HandSubscript, format, false);
            }
        }

        public void Subscribe(ISubscriber subscriber)
        {
            subscribers.Add(subscriber);
        }

        public void Unsubscribe(ISubscriber subscriber)
        {
            subscribers.Remove(subscriber);
        }

        public void OnDestroy()
        {
            saveData.Invalidate();
        }

        public bool AllowedToAdd()
        {
            return true;
        }

        public bool AllowedToRemove()
        {
            return true;
        }

        public bool Empty()
        {
            return fluidStack.amount <= 0;
        }

        public FluidStack Split(int amount, bool simulate = false)
        {
            if (Empty())
            {
                return FluidStack.Empty;
            }

            Remove(amount, out int removed, simulate);

            return new()
            {
                techType = fluidStack.techType,
                amount = removed
            };
        }

        public void Add(int amount, out int consumed, bool simulate = false)
        {
            consumed = Math.Min(amount, maxAmount - fluidStack.amount);

            if (!simulate)
            {
                fluidStack.amount += consumed;
                OnInput();
            }
        }

        public void Remove(int amount, out int removed, bool simulate = false)
        {
            removed = Math.Min(amount, fluidStack.amount);

            if (!simulate)
            {
                fluidStack.amount -= removed;
                OnOutput();
            }
        }

        public void SetFluidStack(FluidStack stack)
        {
            SetFluidStack(stack.techType, stack.amount);
        }

        public void SetFluidStack(TechType techType, int amount)
        {
            if (amount <= 0
                || !techType.IsFluid())
            {
                return;
            }

            fluidStack.techType = techType;
            fluidStack.amount = Math.Min(maxAmount, amount);
            OnInput();
        }

        public void SetFluidStack()
        {
            SetFluidStack(FluidsBasic.Seawater.TechType, 2900);
        }

        private void OnInput()
        {
            foreach (ISubscriber subscriber in subscribers)
            {
                subscriber.OnFluidInput();
            }
        }

        private void OnOutput()
        {
            foreach (ISubscriber subscriber in subscribers)
            {
                subscriber.OnFluidOutput();
            }
        }

        public interface ISubscriber
        {
            public void OnFluidInput();
            public void OnFluidOutput();
        }

        public class SaveData : ComponentSaveData<SaveData, FluidTank>
        {
            public TechType techType;
            public int amount;

            public SaveData(FluidTank component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.fluidTankSaveData;

            public override void CopyFromStorage(SaveData data)
            {
                techType = data.techType;
                amount = data.amount;
            }

            public override void Load()
            {
                Component.SetFluidStack(techType, amount);
            }

            public override void Save()
            {
                techType = Component.fluidStack.techType;
                amount = Component.fluidStack.amount;
            }

            public override bool IncludeInSave()
            {
                return techType != default && amount > 0;
            }
        }
    }
}
