using Industrica.Save;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.World.OreVein
{
    public abstract class AbstractOreVein : WorldObject
    {
        public float RangeSquared => Range * Range;

        public abstract TechType ResourceTechType { get; }
        public abstract TechType OreVeinTechType { get; }
        public abstract TechType CoreSampleTechType { get; }
        public abstract float Range { get; }

        protected static void Setup(
            PrefabInfo info,
            float range,
            IDictionary<BiomeType, WorldUtil.BiomeValidator> biomeSpawnData)
        {
            WorldUtil.AddSpawner(new OreVeinSpawner(info.ClassID, range, biomeSpawnData));
        }

        protected static void Setup<T>(GameObject prefab, PrefabInfo info, float range) where T : AbstractOreVein
        {
            PrefabUtils.AddBasicComponents(prefab, info.ClassID, info.TechType, CellLevel);

            prefab.EnsureComponent<ResourceTracker>().prefabIdentifier = prefab.GetComponent<PrefabIdentifier>();

            prefab.EnsureComponent<T>();
        }

        protected static GameObject GetGameObject<T>(PrefabInfo info, float range) where T : AbstractOreVein
        {
            GameObject prefab = new GameObject();
            prefab.SetActive(false);

            Setup<TitaniumCopperOreVein>(prefab, info, range);

            return prefab;
        }

        public static bool TryFindIntersecting(Vector3 position, out AbstractOreVein result)
        {
            return WorldUtil.TryFindNearestWorldObject(OreVeinHolder.Instance.AllOreVeins, position, out result, PointValidator);
        }

        private static bool PointValidator(WorldUtil.SearchHit<AbstractOreVein> oreVeinHit)
        {
            return oreVeinHit.WorldObject.RangeSquared >= oreVeinHit.SquaredDistance;
        }

        private static bool Intersecting(float thisRange, float otherRange, float squaredDistance)
        {
            float addedRanges = thisRange + otherRange;
            return addedRanges * addedRanges >= squaredDistance;
        }

        public void OnEnable()
        {
            OreVeinHolder.Instance.AllOreVeins.Add(this);
        }

        public void OnDisable()
        {
            OreVeinHolder.Instance.AllOreVeins.Remove(this);
        }

        public const LargeWorldEntity.CellLevel CellLevel = LargeWorldEntity.CellLevel.Medium;
        public const float SafetyDistance = 7f;

        private record OreVeinSpawner(string ClassID, float Range, IDictionary<BiomeType, WorldUtil.BiomeValidator> BiomeSpawnData)
            : WorldUtil.Spawner(ClassID, AbstractOreVein.CellLevel, BiomeSpawnData)
        {
            public override void OnSpawn(in WorldUtil.PositionData positionData)
            {
                OreVeinSaveSystem.Instance.worldgenBlacklist.Add(new(positionData.worldPosition, Range));
                OreVeinSaveSystem.Instance.IncreaseSpawnCount(positionData.biomeType);
            }

            public override bool CanSpawn(in WorldUtil.PositionData positionData)
            {
                float virtualRange = SafetyDistance + Range;
                foreach (OreVeinSaveSystem.OreVein oreVein in OreVeinSaveSystem.Instance.worldgenBlacklist)
                {
                    if (Intersecting(virtualRange, oreVein.Range, Vector3.SqrMagnitude(positionData.worldPosition - oreVein.Position)))
                    {
                        return false;
                    }
                }

                return true;
            }

            public override bool ValidEntitySlot(IEntitySlot entitySlot)
            {
                return entitySlot.IsTypeAllowed(EntitySlot.Type.Small);
            }
        }

        protected record BiomeOreValidator(BiomeType BiomeType, int MaxCount, float Chance)
            : WorldUtil.BiomeSpawnCapValidator(() => OreVeinSaveSystem.Instance.spawnCount, BiomeType, MaxCount, Chance);
    }
}
