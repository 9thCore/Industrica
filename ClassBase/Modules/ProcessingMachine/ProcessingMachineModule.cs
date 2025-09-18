using System;
using UnityEngine;

namespace Industrica.ClassBase.Modules.ProcessingMachine
{
    public abstract class ProcessingMachineModule : MonoBehaviour
    {
        public Interaction interaction;
        public BaseProcessingMachine holder;

        public void SetHolder(BaseProcessingMachine holder)
        {
            this.holder = holder;
        }

        public void SetInteraction(Interaction interaction)
        {
            this.interaction = interaction;
        }

        public abstract void OnStart();
        public abstract void Use(bool append, int index);
        public abstract bool IsEmpty();

        [Flags]
        public enum Interaction
        {
            NoInteraction = 0,
            AllowInput = 1 << 0,
            AllowOutput = 1 << 1,
            DisableUse = 1 << 2,
            LookupRecipeOnInput = 1 << 3,

            InputContainer = AllowInput | AllowOutput | LookupRecipeOnInput,
            OutputContainer = AllowOutput,
            HiddenContainer = NoInteraction | DisableUse
        }
    }
}
