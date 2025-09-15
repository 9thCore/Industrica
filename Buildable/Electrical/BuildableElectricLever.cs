using Industrica.Network.Wire;
using Industrica.Network.Wire.Output;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Electrical
{
    public class BuildableElectricLever
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaElectricLever", true)
                .WithIcon(SpriteManager.Get(TechType.Radio));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, "4f045c69-1539-4c53-b157-767df47c1aa6");

            template.ModifyPrefab += (GameObject obj) =>
            {
                obj.EnsureComponent<DelayedStart>();

                Renderer renderer = obj.GetComponentInChildren<Renderer>();

                PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Inside | ConstructableFlags.Wall, renderer.gameObject);
                
                Vector3 offset = -Vector3.up * 0.33f;
                obj.SetupConstructableBounds(offset: offset);

                foreach (Transform child in obj.transform)
                {
                    child.position += offset;
                }

                Material indicatorMaterial = new(MaterialUtils.Shaders.MarmosetUBER);
                MaterialUtils.ApplyUBERShader(indicatorMaterial, 2f, 0.9f, 1f, MaterialUtils.MaterialType.Opaque);
                GameObject mesh = renderer.gameObject;

                mesh.CreateChild("indicator1", PrimitiveType.Cylinder, withCollider: false)
                .WithSharedMaterial(indicatorMaterial)
                .transform
                .WithLocalPosition(new Vector3(-0.15f, -0.22f, 0.55f))
                .WithScale(0.05f, 0.05f, 0.05f)
                .GetComponent(out Renderer indicator1);

                mesh.CreateChild("indicator2", PrimitiveType.Sphere, withCollider: false)
                .WithSharedMaterial(indicatorMaterial)
                .transform
                .WithLocalPosition(new Vector3(-0.15f, -0.27f, 0.55f))
                .WithScale(0.05f, 0.05f, 0.05f)
                .GetComponent(out Renderer indicator2);

                obj.GetComponent<SkyApplier>().renderers = obj.GetComponentsInChildren<Renderer>(true);

                WirePort output = WirePort.CreatePort(
                    obj,
                    Vector3.up * 0.25f + Vector3.forward * 0.13f - Vector3.right * 0.25f,
                    Quaternion.Euler(0f, 0f, 90f),
                    Network.PortType.Output);

                obj.AddComponent<LeverWireOutput>()
                .WithHandTarget(obj.EnsureComponent<GenericHandTarget>())
                .WithRenderers(indicator1, indicator2)
                .SetPort(output);
            };

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 1),
                new Ingredient(TechType.Copper, 1)
                ));
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
