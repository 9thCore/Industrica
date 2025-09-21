using Industrica.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Utility
{
    public static class WorldUtil
    {
        public static bool TryFindNearestWorldObject<T>(ICollection<T> allWorldObjects, Vector3 position, out T result, Func<SearchHit<T>, bool> validator = null) where T : IFindableObject
        {
            result = default;
            float currentBestDistance = float.MaxValue;

            foreach (T worldObject in allWorldObjects)
            {
                float distance = Vector3.SqrMagnitude(worldObject.Position - position);

                if (distance > currentBestDistance)
                {
                    continue;
                }

                if (validator != null
                    && !validator.Invoke(new SearchHit<T>(worldObject, distance)))
                {
                    continue;
                }

                result = worldObject;
                currentBestDistance = distance;
            }

            return result != null;
        }

        public static void AddSpawner(Spawner spawner)
        {
            Spawners.Add(spawner);
        }

        public static void RunSpawners(IEntitySlot slot, GameObject virtualEntityPrefab, Transform parent, Vector3 localPosition, Quaternion localRotation)
        {
            foreach (Spawner spawner in Spawners)
            {
                if (spawner.Valid(slot))
                {
                    spawner.Spawn(virtualEntityPrefab, slot.GetBiomeType(), parent, localPosition, localRotation);
                }
            }
        }

        private static readonly List<Spawner> Spawners = new();

        public abstract record Spawner(
            string ClassID,
            LargeWorldEntity.CellLevel CellLevel,
            Dictionary<BiomeType, BiomeValidator> BiomeSpawnData)
        {
            public void Register()
            {
                AddSpawner(this);
            }

            public bool Valid(IEntitySlot entitySlot)
            {
                return ValidEntitySlot(entitySlot)
                    && BiomeSpawnData.TryGetValue(entitySlot.GetBiomeType(), out BiomeValidator biomeData)
                    && biomeData.CanSpawn();
            }

            public void Spawn(GameObject virtualEntityPrefab, BiomeType biomeType, Transform parent, Vector3 localPosition, Quaternion localRotation)
            {
                PositionData positionData = new(biomeType, parent, localPosition, localRotation);

                if (!CanSpawn(in positionData))
                {
                    return;
                }

                OnSpawn(in positionData);

                GameObject virtualEntity = UWE.Utils.InstantiateDeactivated(virtualEntityPrefab, parent, localPosition, localRotation);

                virtualEntity.GetComponent<VirtualPrefabIdentifier>().ClassId = ClassID;

                LargeWorldEntity largeWorldEntity = virtualEntity.GetComponent<LargeWorldEntity>();
                largeWorldEntity.cellLevel = CellLevel;

                if (LargeWorld.main != null)
                {
                    virtualEntity.SetActive(LargeWorld.main.streamer.cellManager.RegisterEntity(largeWorldEntity));
                }
            }

            public abstract void OnSpawn(in PositionData positionData);
            public abstract bool CanSpawn(in PositionData positionData);
            public abstract bool ValidEntitySlot(IEntitySlot entitySlot);
        }

        public record SearchHit<T>(T WorldObject, float SquaredDistance) where T : IFindableObject;

        public readonly struct PositionData
        {
            public readonly BiomeType biomeType;
            public readonly Transform parent;
            public readonly Vector3 localPosition;
            public readonly Quaternion localRotation;
            public readonly Vector3 worldPosition;
            public readonly Quaternion worldRotation;

            public PositionData(BiomeType biomeType, Transform parent, Vector3 localPosition, Quaternion localRotation)
            {
                this.biomeType = biomeType;
                this.parent = parent;
                this.localPosition = localPosition;
                this.localRotation = localRotation;

                worldPosition = parent.TransformPoint(localPosition);
                worldRotation = parent.rotation * localRotation;
            }
        }

        public abstract record BiomeValidator
        {
            public abstract bool CanSpawn();
        }

        public record BiomeChanceValidator(float Chance) : BiomeValidator
        {
            public override bool CanSpawn()
            {
                return UnityEngine.Random.value <= Chance;
            }
        }

        public record BiomeSpawnCapValidator(Func<int> CurrentSpawnCount, BiomeType BiomeType, int MaxCount, float Chance)
            : BiomeChanceValidator(Chance)
        {
            public override bool CanSpawn()
            {
                if (!base.CanSpawn())
                {
                    return false;
                }

                return CurrentSpawnCount() < MaxCount;
            }
        }
    }
}
