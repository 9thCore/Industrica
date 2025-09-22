using Industrica.Machine.Mining;
using Industrica.Network;
using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Pipe.Item;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Linq;
using UnityEngine;
using static Industrica.Recipe.Handler.CrusherRecipeHandler.Recipe;

namespace Industrica.Buildable.Mining
{
    public static class BuildableDrill
    {
        public const int Width = 2;
        public const int Height = 2;

        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaDrill")
                .WithIcon(SpriteManager.Get(TechType.ThermalPlant));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, TechType.ThermalPlant);

            template.ModifyPrefab += ModifyPrefab;

            prefab.SetRecipe(new RecipeData()
            {
                Ingredients = new()
                {
                    new Ingredient(TechType.TitaniumIngot, 1),
                    new Ingredient(TechType.ComputerChip, 1)
                }
            });

            prefab.SetPdaGroupCategory(TechGroup.ExteriorModules, TechCategory.ExteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }

        private static void ModifyPrefab(GameObject go)
        {
            go.EnsureComponent<DelayedStart>();
            go.EnsureComponent<StorageContainerProvider>();

            PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

            ThermalPlantModel model = go.GetComponentInChildren<ThermalPlantModel>(true);
            model.gameObject.EnsureComponent<DrillModel>();

            GameObject.DestroyImmediate(go.FindChild("model/root/head"));
            GameObject.DestroyImmediate(go.FindChild("Sphere"));
            GameObject.DestroyImmediate(go.FindChild("UI"));
            GameObject.DestroyImmediate(go.GetComponent<LiveMixin>());
            GameObject.DestroyImmediate(go.GetComponent<ThermalPlant>());
            GameObject.DestroyImmediate(go.GetComponent<PowerSource>());
            GameObject.DestroyImmediate(go.GetComponent<PowerRelay>());
            GameObject.DestroyImmediate(go.GetComponent<PowerSystemPreview>());
            GameObject.DestroyImmediate(model);
            GameObject.DestroyImmediate(go.GetComponents<PowerFX>().First(fx => fx.vfxPrefab.name.Contains("Preview")));

            go.EnsureComponent<Drill>()
                .WithHandTarget(go.EnsureComponent<GenericHandTarget>())
                .WithStorageContainer(PrefabUtils.AddStorageContainer(go, "StorageRoot", "StorageRoot", Width, Height))
                .GatherColliders();

            TransferItemPort.CreatePort(
                prefab: go,
                root: go,
                position: Vector3.up * 3.25f,
                rotation: Quaternion.identity,
                PortType.Output);
        }
    }
}
