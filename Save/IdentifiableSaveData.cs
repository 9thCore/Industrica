using Newtonsoft.Json;

namespace Industrica.Save
{
    public abstract class IdentifiableSaveData<S> : AbstractSaveData<S> where S : AbstractSaveData<S>
    {
        [JsonIgnore]
        private UniqueIdentifier uniqueIdentifier;
        [JsonIgnore]
        public UniqueIdentifier SaveDataIdentifier
        {
            get
            {
                if (uniqueIdentifier == null)
                {
                    uniqueIdentifier = GetUniqueIdentifier;
                }
                return uniqueIdentifier;
            }
        }

        public override string SaveKey => SaveDataIdentifier.Id;
        [JsonIgnore]
        public abstract UniqueIdentifier GetUniqueIdentifier { get; }
    }
}
