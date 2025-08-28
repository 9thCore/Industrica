using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Item.Mining.CoreSample
{
    public static class ItemCoreSampleTitaniumCopper
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaCoreSampleTitaniumCopper")
                .WithIcon(SpriteManager.Get(TechType.LabContainer3));

            var prefab = new CustomPrefab(Info);
            var template = new CloneTemplate(Info, TechType.LabContainer3);

            template.ModifyPrefab += ModifyPrefab;

            prefab.SetGameObject(template);
            prefab.Register();
        }

        private static void ModifyPrefab(GameObject prefab)
        {
            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Near);
        }
    }
}
