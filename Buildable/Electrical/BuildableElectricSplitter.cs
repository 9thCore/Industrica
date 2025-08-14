using Industrica.Network.Wire;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Electrical
{
    public class BuildableElectricSplitter
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaElectricSplitter", true)
                .WithIcon(SpriteManager.Get(TechType.Locker));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, "4bc83dc1-dd91-4478-9b35-fd520ccaeb7c");

            template.ModifyPrefab += (GameObject obj) =>
            {
                Renderer renderer = obj.GetComponentInChildren<Renderer>();

                PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Inside | ConstructableFlags.Wall, renderer.gameObject);

                obj.SetupConstructableBounds();

                WirePort input = WirePort.CreatePort(
                    obj,
                    new Vector3(-0.08f, -0.06f, 0.13f),
                    Quaternion.Euler(90f, 0f, 0f),
                    Network.PortType.Input);

                Vector3 side = Vector3.right * 0.4f;
                Vector3 offset = Vector3.forward * 0.03f;

                WirePort output1 = WirePort.CreatePort(
                    obj,
                    offset + side,
                    Quaternion.Euler(0f, 0f, 270f),
                    Network.PortType.Output);

                WirePort output2 = WirePort.CreatePort(
                    obj,
                    offset - side,
                    Quaternion.Euler(0f, 0f, 90f),
                    Network.PortType.Output);

                obj.EnsureComponent<WireSplitter>().SetPorts(input, output1, output2);
            };

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.WiringKit, 1)
                ));
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
