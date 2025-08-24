using Industrica.Machine.FilterWriter;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Storage
{
    public static class BuildabltemTechTypeFilterWriter
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaItemTechTypeFilterWriter", true)
                .WithIcon(SpriteManager.Get(TechType.Workbench));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, TechType.Workbench);

            template.ModifyPrefab += (GameObject go) =>
            {
                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

                ChildObjectIdentifier identifier = go.CreateChild("StorageRoot")
                .EnsureComponent<ChildObjectIdentifier>();
                identifier.classId = identifier.gameObject.name;

                GameObject handTarget = go.CreateChild(nameof(HandTarget))
                .transform
                .WithScale(x: 1.35f, y: 1.05f, z: 1.05f)
                .WithLocalPosition(Vector3.up * 0.5f)
                .gameObject;

                handTarget.EnsureComponent<BoxCollider>();

                WriterItemTechTypeFilter filterStorage = go.EnsureComponent<WriterItemTechTypeFilter>()
                .WithStorageRoot(identifier.transform)
                .WithHandTarget(handTarget.EnsureComponent<GenericHandTarget>())
                .WithWorkbench(go.GetComponent<Workbench>());
            };

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.ComputerChip, 1)
                ));

            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
