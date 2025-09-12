using Industrica.World.OreVein;
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
        public Dictionary<AbstractOreVein.OreVeinType, Dictionary<BiomeType, int>> spawnCount = new();

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

        public void IncreaseSpawnCount(AbstractOreVein.OreVeinType oreVeinType, BiomeType biomeType)
        {
            if (!spawnCount.ContainsKey(oreVeinType))
            {
                spawnCount[oreVeinType] = new()
                {
                    { biomeType, 1 }
                };

                return;
            }

            if (!spawnCount[oreVeinType].ContainsKey(biomeType))
            {
                spawnCount[oreVeinType][biomeType] = 1;
                return;
            }

            spawnCount[oreVeinType][biomeType]++;
        }

        public int GetSpawnCount(AbstractOreVein.OreVeinType oreVeinType, BiomeType biomeType)
        {
            if (spawnCount.TryGetValue(oreVeinType, out Dictionary<BiomeType, int> instance)
                && instance.TryGetValue(biomeType, out int result))
            {
                return result;
            }

            return 0;
        }

        public record OreVein(Vector3 Position, float Range);
    }
}
