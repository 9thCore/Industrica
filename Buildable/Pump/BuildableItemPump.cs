using Industrica.Network;
using Industrica.Network.Container.Provider.Item.Industrica;
using Industrica.Network.Physical.Item;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Pump
{
    public class BuildableItemPump
    {
        public static readonly Texture Texture = PathUtil.GetTexture("Pump/monitor");

        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaItemPump", true)
                .WithIcon(SpriteManager.Get(TechType.PropulsionCannon));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, "0782292e-d313-468a-8816-2adba65bfba3");

            template.ModifyPrefab += (GameObject obj) =>
            {
                Renderer renderer = obj.GetComponentInChildren<Renderer>();

                PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Inside | ConstructableFlags.Wall, renderer.gameObject);

                obj.EnsureComponent<PhysicalNetworkItemPump>().WithHandTarget(obj.EnsureComponent<GenericHandTarget>());
                obj.EnsureComponent<ItemPumpContainerProvider>();

                Vector3 offset = -Vector3.up * 0.25f;
                obj.SetupConstructableBounds(offset: offset);

                foreach (Transform child in obj.transform)
                {
                    child.position += offset;
                }

                PhysicalNetworkItemPort.CreatePort(
                    prefab: obj,
                    root: obj,
                    Vector3.right * 0.4f,
                    Quaternion.Euler(0f, 0f, 270f),
                    PortType.Input);

                PhysicalNetworkItemPort.CreatePort(
                    prefab: obj,
                    root: obj,
                    Vector3.right * -0.4f,
                    Quaternion.Euler(0f, 0f, 90f),
                    PortType.Output);

                renderer.materials[0].SetFloat("_LightmapStrength", 1f);
                renderer.materials[1].SetTexture("_Illum", Texture);
                renderer.materials[1].SetTexture("_SpecTex", Texture);
                renderer.materials[1].mainTexture = Texture;
            };

            prefab.SetRecipe(new RecipeData(
                new CraftData.Ingredient(TechType.Titanium, 2),
                new CraftData.Ingredient(TechType.WiringKit)
                ));
            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
