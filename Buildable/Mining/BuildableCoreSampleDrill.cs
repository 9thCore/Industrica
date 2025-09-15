using Industrica.Machine.Mining;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Linq;
using UnityEngine;

namespace Industrica.Buildable.Mining
{
    public static class BuildableCoreSampleDrill
    {
        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaCoreSampleDrill")
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

            PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

            ThermalPlantModel model = go.GetComponentInChildren<ThermalPlantModel>(true);
            model.gameObject.EnsureComponent<CoreSampleDrillModel>();

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

            go.EnsureComponent<CoreSampleDrill>()
                .WithHandTarget(go.EnsureComponent<GenericHandTarget>())
                .GatherColliders();
        }
    }
}
