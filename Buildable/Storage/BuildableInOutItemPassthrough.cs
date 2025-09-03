using Industrica.Network.Container.Provider.Item.Industrica;
using Industrica.Network.Pipe.Item;
using Industrica.Storage.Passthrough;
using Industrica.Utility;
using ModularilyBased.API.Buildable;
using ModularilyBased.API.Buildable.PlaceRule;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Buildable.Storage
{
    public static class BuildableInOutItemPassthrough
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaInOutItemPassthrough", true)
                .WithIcon(SpriteManager.Get(TechType.PlanterPot3));

            CustomPrefab prefab = new(Info);

            prefab.SetRecipe(new RecipeData(
                new Ingredient(TechType.Titanium, 2),
                new Ingredient(TechType.Lead, 1)
                ));

            prefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);

            prefab.SetGameObject(GetGameObject);
            prefab.Register();
        }

        private static IEnumerator GetGameObject(IOut<GameObject> result)
        {
            GameObject prefab = new GameObject(Info.ClassID);
            prefab.SetActive(false);

            result.Set(prefab);

            GameObject inOut = prefab.CreateChild(nameof(inOut));
            inOut.AddComponent<InOutItemPassthrough>();
            inOut.AddComponent<InOutItemPassthroughProvider>();
            inOut.AddComponent<ChildObjectIdentifier>().classId = "InOut";

            GameObject outIn = prefab.CreateChild(nameof(outIn));
            outIn.AddComponent<InOutItemPassthrough>();
            outIn.AddComponent<InOutItemPassthroughProvider>();
            outIn.AddComponent<ChildObjectIdentifier>().classId = "OutIn";

            GameObject model = prefab.CreateChild(nameof(model));
            GameObject collision = prefab.CreateChild(nameof(collision));

            ModuleSnapper.SetSnappingRules(
                PrefabUtils.AddConstructable(prefab, Info.TechType, ConstructableFlags.None, model),
                ModuleSnapper.RoomRule.Corridor,
                PlacementRule.SnapToCorridorCap);

            yield return CreateInside(prefab, model, collision);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: inOut,
                InOutOffset - Vector3.forward * 0.1f,
                Quaternion.Euler(90f, 0f, 0f),
                Network.PortType.Input);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: inOut,
                InOutOffset + new Vector3(0f, 0f, -0.9f),
                Quaternion.Euler(275f, 0f, 0f),
                Network.PortType.Output,
                true);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: outIn,
                OutInOffset + new Vector3(0f, 0f, -0.9f),
                Quaternion.Euler(265f, 0f, 0f),
                Network.PortType.Input,
                true);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: outIn,
                OutInOffset - Vector3.forward * 0.1f,
                Quaternion.Euler(90f, 0f, 0f),
                Network.PortType.Output);

            // Give inside renderers to SkyApplier, but not the outside renderers
            // (This method adds the SkyApplier automatically)
            PrefabUtils.AddBasicComponents(prefab, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);

            yield return CreateOutside(prefab, model, collision);

            GameObjectUtil.SetupConstructableBounds(prefab);
        }

        private static IEnumerator CreateInside(GameObject prefab, GameObject model, GameObject collision)
        {
            IPrefabRequest request = PrefabDatabase.GetPrefabAsync("2cfce053-3509-49e0-9e2a-e981436dc9ad");
            yield return request;
            
            if (!request.TryGetPrefab(out GameObject vanillaPrefab))
            {
                yield break;
            }

            GameObject vanillaModel = vanillaPrefab.FindChild("Base_interior_Planter_Pot_03");
            GameObject vanillaCollision = vanillaPrefab.FindChild("Cylinder");

            InstantiateHelper(model.transform, vanillaModel, InsidePosition, InsideRotation, Scale);
            InstantiateHelper(collision.transform, vanillaCollision, InsidePosition, InsideRotation, Scale);
        }

        private static IEnumerator CreateOutside(GameObject prefab, GameObject model, GameObject collision)
        {
            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.BasePipeConnector, false);
            yield return task;

            GameObject vanillaPrefab = task.GetResult();

            GameObject vanillaModel = vanillaPrefab.GetComponentInChildren<MeshCollider>().gameObject;
            GameObject vanillaCollision = vanillaPrefab.FindChild("collider");

            InstantiateHelper(model.transform, vanillaModel, OutsidePosition, OutsideRotationTop, OutsideRotationBottom, Scale);
            InstantiateHelper(collision.transform, vanillaCollision, OutsidePosition, OutsideRotationTop, OutsideRotationBottom, Scale);
        }

        private static void InstantiateHelper(Transform parent, GameObject vanillaPart, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            InstantiateHelper(parent, vanillaPart, position, rotation, rotation, scale);
        }

        private static void InstantiateHelper(Transform parent, GameObject vanillaPart, Vector3 position, Quaternion rotation1, Quaternion rotation2, Vector3 scale)
        {
            GameObject.Instantiate(vanillaPart)
                .transform
                .WithParent(parent)
                .WithLocalPosition(position + InOutOffset)
                .WithLocalRotation(rotation1)
                .WithScale(scale);

            GameObject.Instantiate(vanillaPart)
                .transform
                .WithParent(parent)
                .WithLocalPosition(position + OutInOffset)
                .WithLocalRotation(rotation2)
                .WithScale(scale);
        }

        public static readonly Vector3 Scale = Vector3.one * 0.5f;

        public static readonly Quaternion InsideRotation = Quaternion.Euler(0f, 0f, 90f);
        public static readonly Vector3 InsidePosition = Vector3.forward * -0.22f;

        public static readonly Quaternion OutsideRotationTop = Quaternion.Euler(0f, 270f, 265f);
        public static readonly Quaternion OutsideRotationBottom = Quaternion.Euler(0f, 270f, 275f);
        public static readonly Vector3 OutsidePosition = Vector3.forward * -0.9f;

        public static readonly Vector3 InOutOffset = Vector3.up * 0.33f;
        public static readonly Vector3 OutInOffset = -InOutOffset;
    }
}
