using Industrica.Network;
using Industrica.Network.Container.Provider;
using Industrica.Network.Physical;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using static CraftData;

namespace Industrica.Buildable.ConnectableTest
{
    public static class ConnectorTest
    {
        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaConnection", true)
                .WithIcon(SpriteManager.Get(TechType.SmallLocker));

            var prefab = new CustomPrefab(Info);

            prefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);
            prefab.SetRecipe(
                new RecipeData(
                    new Ingredient(TechType.Titanium))
                ).WithCraftingTime(5f);

            var template = new CloneTemplate(Info, "51afa357-8ec9-4f45-916f-6998a7c35314");

            template.ModifyPrefab += obj =>
            {
                GameObject model = obj.GetComponentInChildren<Renderer>().gameObject;

                PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Base | ConstructableFlags.Rotatable | ConstructableFlags.Ground, model);
                
                PrefabUtils.AddStorageContainer(obj, "ConnectionStorage", "ConnectionStorageClassId", 4, 4);
                obj.EnsureComponent<StorageContainerProvider>();

                PhysicalNetworkItemPort.CreatePort(obj, Vector3.up * 1.05f + Vector3.forward, Quaternion.identity, PortType.Input);
                PhysicalNetworkItemPort.CreatePort(obj, Vector3.up * 1.05f - Vector3.forward, Quaternion.identity, PortType.Output);
            };

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
