using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Pipe.Item;
using Industrica.Network.Wire;
using Industrica.Network.Wire.Output;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Storage
{
    public static class BuildableWeighedItemLocker
    {
        public const int Width = 5;
        public const int Height = 5;

        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaWeighedItemLocker", true)
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

                StorageContainer container = go.GetComponent<StorageContainer>();
                container.width = Width;
                container.height = Height;

                go.EnsureComponent<StorageContainerProvider>();

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

                WirePort output = WirePort.CreatePort(
                    go,
                    Vector3.forward * 0.35f - Vector3.up * 0.17f,
                    Quaternion.Euler(90f, 0f, 0f),
                    Network.PortType.Output);
                
                go.EnsureComponent<ItemContainerWireOutput>().SetPort(output);
            };

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.WiringKit, 1),
                new Ingredient(TechType.CopperWire, 1)
                ));

            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
