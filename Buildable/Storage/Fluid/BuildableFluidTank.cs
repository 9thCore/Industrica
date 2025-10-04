using Industrica.Fluid;
using Industrica.Network.Container.Provider.Fluid.Industrica;
using Industrica.Network.Pipe.Fluid;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Storage.Fluid
{
    public static class BuildableFluidTank
    {
        public static PrefabInfo Info { get; private set; }

        public static int MaxAmount = 5000;

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaFluidTank")
                .WithIcon(SpriteManager.Get(TechType.Aquarium));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, TechType.Aquarium);

            template.ModifyPrefab += (GameObject go) =>
            {
                GameObject.DestroyImmediate(go.GetComponent<Aquarium>());
                GameObject.DestroyImmediate(go.GetComponent<StorageContainer>().storageRoot.gameObject);
                GameObject.DestroyImmediate(go.GetComponent<StorageContainer>());

                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

                Vector3 bottomAndFront = Vector3.up * 0.2f + Vector3.forward * 0.4f;
                Vector3 rightSide = Vector3.right * 1.0f;
                Vector3 leftSide = -rightSide;

                TransferFluidPort.CreatePort(
                    prefab: go,
                    root: go,
                    leftSide + bottomAndFront,
                    Quaternion.Euler(90f, 0f, 0f),
                    Network.PortType.Input);

                TransferFluidPort.CreatePort(
                    prefab: go,
                    root: go,
                    rightSide + bottomAndFront,
                    Quaternion.Euler(90f, 0f, 0f),
                    Network.PortType.Output);

                go.EnsureComponent<FluidTank>()
                .WithMaxAmount(MaxAmount)
                .SetHandTarget(go.EnsureComponent<GenericHandTarget>());

                go.EnsureComponent<FluidTankContainerProvider>();
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
