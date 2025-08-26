using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Physical.Item;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Storage
{
    public static class BuildableBigItemLocker
    {
        public const int Width = 6;
        public const int Height = 6;

        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaBigItemLocker", true)
                .WithIcon(SpriteManager.Get(TechType.Locker));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, TechType.Locker);

            template.ModifyPrefab += (GameObject go) =>
            {
                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

                StorageContainer container = go.GetComponent<StorageContainer>();
                container.width = Width;
                container.height = Height;

                go.EnsureComponent<StorageContainerProvider>();

                Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
                Vector3 front = Vector3.forward * 0.25f;
                Vector3 topSection = Vector3.up * 1.48f;
                Vector3 bottomSection = Vector3.up * 0.39f;
                Vector3 rightSide = Vector3.right * -0.17f;
                Vector3 leftSide = -rightSide;

                PhysicalNetworkItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    rightSide + topSection + front,
                    rotation,
                    Network.PortType.Input);

                PhysicalNetworkItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    leftSide + topSection + front,
                    rotation,
                    Network.PortType.Input);

                PhysicalNetworkItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    rightSide + bottomSection + front,
                    rotation,
                    Network.PortType.Output);

                PhysicalNetworkItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    leftSide + bottomSection + front,
                    rotation,
                    Network.PortType.Output);
            };

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Quartz, 2),
                new Ingredient(TechType.Titanium, 3)
                ));

            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
