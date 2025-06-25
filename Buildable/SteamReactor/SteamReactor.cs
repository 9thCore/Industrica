using Industrica.Utility;
using ModularilyBased.Library;
using ModularilyBased.Library.PlaceRule;
using ModularilyBased.Library.TransformRule;
using ModularilyBased.Library.TransformRule.Rotation;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Utility;
using System;
using UnityEngine;
using Ingredient = CraftData.Ingredient;
using RecipeData = Nautilus.Crafting.RecipeData;

namespace Industrica.Buildable.SteamReactor
{
    public class SteamReactor
    {
        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaSteamReactor", true)
                .WithIcon(SpriteManager.Get(TechType.BaseBioReactor));

            var prefab = new CustomPrefab(Info);

            prefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);
            prefab.SetRecipe(
                new RecipeData(
                    new Ingredient(TechType.PlasteelIngot),
                    new Ingredient(TechType.Lubricant, 2),
                    new Ingredient(TechType.AdvancedWiringKit))
                ).WithCraftingTime(5f);

            prefab.SetGameObject(GetGameObject);
            prefab.Register();
        }

        public static GameObject GetGameObject()
        {
            if (!GameObjectUtil.TryFindObjectWith(model =>
            {
                return !model.name.Contains("Dome");
            }, out BaseBioReactorGeometry geometry))
            {
                throw new InvalidOperationException();
            }

            GameObject prefab = GameObject.Instantiate(geometry.gameObject);
            prefab.SetActive(false);
            prefab.name = Info.TechType.ToString();

            prefab.EnsureComponent<SteamReactorBehaviour>();
            GameObject.DestroyImmediate(prefab.GetComponent<BaseBioReactorGeometry>());

            Transform model = prefab.transform.Find("Bio_reactor");

            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

            Constructable constructable = PrefabUtils.AddConstructable(
                    prefab,
                    Info.TechType,
                    ConstructableFlags.Rotatable,
                    model.gameObject);

            foreach (ConstructableBounds bounds in prefab.GetComponents<ConstructableBounds>())
            {
                bounds.bounds.size *= 0.9f;
            }

            TransformationRule rule = new TransformationRule()
                .WithRotationRule(SnappedRotationRule.NoOffsetCardinal);

            ModuleSnapper.SetSnappingRules(
                constructable,
                ModuleSnapper.RoomRule.SmallRoom | ModuleSnapper.RoomRule.LargeRoom,
                PlacementRule.SnapToCenter,
                rule);

            MaterialUtils.ApplySNShaders(model.gameObject);

            return prefab;
        }
    }
}
