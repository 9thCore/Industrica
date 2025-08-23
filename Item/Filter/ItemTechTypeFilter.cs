using Industrica.Network;
using Industrica.Network.Filter.Holder;
using Industrica.Register.Equipment;
using Industrica.UI.Overlay.Holder;
using Industrica.UI.Tooltip;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Item.Filter
{
    public static class ItemTechTypeFilter
    {
        public static PrefabInfo Info { get; private set; }

        private static Sprite _icon;
        public static Sprite Icon => _icon ??= SpriteManager.Get(TechType.FiberMesh);

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaItemTechTypeFilter", true)
                .WithIcon(Icon);

            var prefab = new CustomPrefab(Info);
            var template = new CloneTemplate(Info, TechType.FiberMesh);

            template.ModifyPrefab += (GameObject obj) =>
            {
                PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Near);

                TechTypeNetworkFilterHolder holder = obj.EnsureComponent<TechTypeNetworkFilterHolder>();

                obj.EnsureComponent<ItemTechTypeFilterOverlayHolder>()
                .WithNetworkFilterHolder(holder);

                obj.EnsureComponent<ItemTechTypeFilterTooltip>()
                .WithNetworkFilterHolder(holder);
            };

            prefab.SetRecipe(new Nautilus.Crafting.RecipeData
            {
                Ingredients =
                {
                    new Ingredient(TechType.FiberMesh, 2),
                    new Ingredient(TechType.Titanium, 2),
                    new Ingredient(TechType.WiringKit, 1)
                }
            });

            prefab.SetGameObject(template);
            prefab.SetEquipment(FilterEquipment.Type);
            prefab.Register();
        }
    }
}
