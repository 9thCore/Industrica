using Nautilus.Assets;
using Nautilus.Utility;
using System;
using System.Collections;
using UnityEngine;

namespace Industrica.Network.Systems
{
    public class ItemPhysicalNetwork : PhysicalNetwork<Pickupable>
    {
        public static PrefabInfo Info { get; private set; }

        public static void RegisterPrefab()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaItemNetwork", false);

            CustomPrefab prefab = new CustomPrefab(Info);
            prefab.SetGameObject(() =>
            {
                GameObject prefab = new GameObject(nameof(ItemPhysicalNetwork));
                prefab.SetActive(false);
                prefab.EnsureComponent<ItemPhysicalNetwork>();
                PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                return prefab;
            });

            prefab.Register();
        }

        public static IEnumerator Create(Action<ItemPhysicalNetwork> postLoadAction)
        {
            yield return CreateNetwork(Info.TechType, postLoadAction);
        }
    }
}
