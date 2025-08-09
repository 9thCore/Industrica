using Industrica.Network.Physical.Item;
using Industrica.Network.Wire;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Industrica.Save
{
    [FileName("save")]
    public class SaveSystem : SaveDataCache
    {
        public static SaveSystem Instance { get; private set; }

        public SaveData<PlacedItemTransferPipe.SaveData> placedItemTransferPipeData = new();
        public SaveData<PhysicalNetworkItemPort.SaveData> physicalNetworkItemPortData = new();
        public SaveData<PlacedWire.SaveData> placedWireData = new();
        public SaveData<WirePort.SaveData> outputWirePortData = new();
        public SaveData<WireLogicGate.SaveData> wireLogicGateData = new();

        private IEnumerable<ISaveData> AllSaveData => typeof(SaveSystem)
            .GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(info => typeof(ISaveData).IsAssignableFrom(info.FieldType))
            .Select(info => info.GetValue(this) as ISaveData);

        public static void Register()
        {
            Instance = SaveDataHandler.RegisterSaveDataCache<SaveSystem>();

            Instance.OnStartedSaving += (obj, args) =>
            {
                SaveSystem instance = args.Instance as SaveSystem;
                instance.AllSaveData.ForEach(data => data.Save());
            };

            Instance.OnFinishedLoading += (obj, args) =>
            {
                SaveSystem instance = args.Instance as SaveSystem;
                instance.AllSaveData.ForEach(data => data.Load());
            };
        }

        public class SaveData<S> : Dictionary<string, S>, ISaveData where S : AbstractSaveData<S>
        {
            private Dictionary<string, S> dirty;

            public bool TryLoad(S saveData)
            {
                bool flag = false;
                if (dirty.TryGetValue(saveData.SaveKey, out S loadData))
                {
                    saveData.CopyFromStorage(loadData);
                    flag = true;
                }

                dirty[saveData.SaveKey] = saveData;
                return flag;
            }

            public void Save()
            {
                Cleanup();

                Clear();
                dirty.ForEach(pair => pair.Value.UpdateSaveIfAble());
                IEnumerable<string> validKeys = dirty.Where(pair => pair.Value.IncludeInSave()).Select(pair => pair.Key);
                validKeys.ForEach(key => Add(key, dirty[key]));
            }

            public void Load()
            {
                dirty = new(this);
                Clear();
            }

            private void Cleanup()
            {
                dirty.ForEach(pair => pair.Value.InvalidateIfNotValid());
                string[] invalidKeys = dirty.Where(pair => !pair.Value.Valid).Select(pair => pair.Key).ToArray();
                invalidKeys.ForEach(key => dirty.Remove(key));
            }
        }

        private interface ISaveData
        {
            public void Save();
            public void Load();
        }
    }
}
