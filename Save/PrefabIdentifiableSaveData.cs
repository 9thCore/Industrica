using Newtonsoft.Json;
using System.Collections.Generic;

namespace Industrica.Save
{
    public abstract class PrefabIdentifiableSaveData : AbstractSaveData
    {
        [JsonIgnore]
        private PrefabIdentifier identifier;
        [JsonIgnore]
        public PrefabIdentifier Identifier => identifier ??= GetIdentifier;
        [JsonIgnore]
        public string Id => Identifier.Id;
        [JsonIgnore]
        public string ClassId => Identifier.ClassId;

        [JsonIgnore]
        public abstract PrefabIdentifier GetIdentifier { get; }

        public void TryLoad<S>(Dictionary<string, S> storage) where S : AbstractSaveData
        {
            base.TryLoad(storage, Id);
        }
    }
}
