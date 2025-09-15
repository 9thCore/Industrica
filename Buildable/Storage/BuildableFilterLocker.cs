using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Pipe.Item;
using Industrica.Storage;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Storage
{
    public static class BuildableFilterLocker
    {
        public const int Width = 4;
        public const int Height = 4;

        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaFilterLocker", true)
                .WithIcon(SpriteManager.Get(TechType.SmallLocker));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, TechType.SmallLocker);

            template.ModifyPrefab += (GameObject go) =>
            {
                go.EnsureComponent<DelayedStart>();

                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

                GameObject.DestroyImmediate(go.GetComponentInChildren<ColoredLabel>());
                GameObject.DestroyImmediate(go.FindChild("TriggerCull"));
                GameObject.DestroyImmediate(go.FindChild("Label"));

                StorageContainer storage = go.GetComponent<StorageContainer>();
                storage.width = Width;
                storage.height = Height;

                go.EnsureComponent<StorageContainerProvider>();

                GameObject filter = go.CreateChild("Filter");

                filter.EnsureComponent<ChildObjectIdentifier>().classId = filter.name;

                Vector3 topAndFront = Vector3.up * 0.42f + Vector3.forward * 0.17f;
                Vector3 rightSide = Vector3.right * -0.42f;
                Vector3 leftSide = -rightSide;

                TransferItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    leftSide + topAndFront,
                    Quaternion.Euler(0f, 0f, 270f),
                    Network.PortType.Input);

                TransferItemPort.CreatePort(
                    prefab: go,
                    root: go,
                    rightSide + topAndFront,
                    Quaternion.Euler(0f, 0f, 90f),
                    Network.PortType.Output);

                ChildObjectIdentifier identifier = filter.CreateChild("FilterStorageRoot")
                .EnsureComponent<ChildObjectIdentifier>();
                identifier.classId = identifier.gameObject.name;

                GameObject handTarget = filter.CreateChild(nameof(HandTarget))
                .transform
                .WithScale(x: 0.55f, y: 0.25f, z: 0.2f)
                .WithLocalPosition(Vector3.up * 0.03f + Vector3.forward * 0.3f)
                .gameObject;

                handTarget.EnsureComponent<BoxCollider>();

                NetworkFilterStorage filterStorage = go.EnsureComponent<NetworkFilterStorage>()
                .WithStorageRoot(identifier.transform)
                .WithHandTarget(handTarget.EnsureComponent<GenericHandTarget>())
                .WithFilteredContainer(go.EnsureComponent<FilteredStorageContainer>());
            };

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 3),
                new Ingredient(TechType.AdvancedWiringKit, 1)
                ));

            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
