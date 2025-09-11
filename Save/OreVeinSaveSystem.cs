using Nautilus.Handlers;
using Nautilus.Json;
using Nautilus.Json.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Save
{
    [FileName("oreVeins")]
    public class OreVeinSaveSystem : SaveDataCache
    {
        public static OreVeinSaveSystem Instance { get; private set; }

        public List<OreVein> worldgenBlacklist = new();
        public Dictionary<BiomeType, int> spawnCount = new();

        public static void Register()
        {
            Instance = SaveDataHandler.RegisterSaveDataCache<OreVeinSaveSystem>();

            Instance.OnStartedLoading += (obj, args) =>
            {
                OreVeinSaveSystem instance = args.Instance as OreVeinSaveSystem;
                instance.worldgenBlacklist.Clear();
                instance.spawnCount.Clear();
            };
        }

        public void IncreaseSpawnCount(BiomeType biomeType)
        {
            if (!spawnCount.ContainsKey(biomeType))
            {
                spawnCount[biomeType] = 1;
                return;
            }

            spawnCount[biomeType]++;
        }

        public record OreVein(Vector3 Position, float Range);
    }
}
