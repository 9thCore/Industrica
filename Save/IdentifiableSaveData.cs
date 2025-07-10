using Newtonsoft.Json;

namespace Industrica.Save
{
    public abstract class IdentifiableSaveData : AbstractSaveData
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
        [JsonIgnore]
        public override string SaveKey => SaveDataIdentifier.Id;
        [JsonIgnore]
        public abstract UniqueIdentifier GetUniqueIdentifier { get; }
    }
}
