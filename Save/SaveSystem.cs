using Industrica.Buildable.SteamReactor;
using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Industrica.Save
{
    [FileName("save")]
    public class SaveSystem : SaveDataCache
    {
        public static SaveSystem Instance { get; private set; }

        public Dictionary<string, SteamReactorBehaviour.SaveData> steamReactorSaveData = new();

        public static void Register()
        {
            Instance = SaveDataHandler.RegisterSaveDataCache<SaveSystem>();

            Instance.OnStartedSaving += (obj, args) =>
            {
                SaveSystem instance = args.Instance as SaveSystem;

                instance.SaveStorage(instance.steamReactorSaveData);
            };
        }

        public void SaveStorage<T>(Dictionary<string, T> storage) where T : AbstractSaveData
        {
            Cleanup(storage);
            storage.ForEach(pair => pair.Value.Save());
        }

        public void Cleanup<T>(Dictionary<string, T> storage) where T : AbstractSaveData
        {
            string[] invalidKeys = storage.Where(pair => !pair.Value.ValidForSaving()).Select(pair => pair.Key).ToArray();
            invalidKeys.ForEach(key => storage.Remove(key));
        }
    }
}
