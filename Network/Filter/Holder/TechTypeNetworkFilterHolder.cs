using Industrica.Network.Filter.Item;
using Industrica.Save;
using UnityEngine.Events;

namespace Industrica.Network.Filter.Holder
{
    public class TechTypeNetworkFilterHolder : NetworkFilterHolder<Pickupable>, IProtoEventListener
    {
        private SaveData saveData;

        // My serialization already works well enough, no need to reinvent the wheel with custom serialization
        private void EnsureSaveData()
        {
            saveData ??= new SaveData(this);
        }

        public void OnProtoSerialize(ProtobufSerializer _) => EnsureSaveData();
        public void OnProtoDeserialize(ProtobufSerializer _) => EnsureSaveData();

        public override NetworkFilter<Pickupable> GetFilter => new TechTypeNetworkFilter();

        public TechType TechType
        {
            get
            {
                return (Filter as TechTypeNetworkFilter)?.techType ?? TechType.None;
            }
            set
            {
                if (Filter is TechTypeNetworkFilter techTypeFilter)
                {
                    techTypeFilter.techType = value;
                }
            }
        }

        public class TechTypeUpdateEvent : UnityEvent<TechType> { }

        public class SaveData : ComponentSaveData<SaveData, TechTypeNetworkFilterHolder>
        {
            public TechType techType;

            public SaveData(TechTypeNetworkFilterHolder component) : base(component) { }

            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.techTypeFilterSaveData;

            public override void CopyFromStorage(SaveData data)
            {
                techType = data.techType;
            }

            public override void Load()
            {
                Component.TechType = techType;
            }

            public override void Save()
            {
                techType = Component.TechType;
            }

            public override bool IncludeInSave()
            {
                return techType != default;
            }
        }
    }
}
