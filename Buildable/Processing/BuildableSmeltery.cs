using Industrica.Machine.Processing;
using Industrica.Network;
using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Pipe.Item;
using Industrica.UI;
using Industrica.UI.UIData;
using Industrica.Utility;
using ModularilyBased.API.Buildable;
using ModularilyBased.API.Buildable.PlaceRule;
using ModularilyBased.API.Buildable.TransformRule;
using ModularilyBased.API.Buildable.TransformRule.Rotation;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using Nautilus.Utility;
using TMPro;
using UnityEngine;

namespace Industrica.Buildable.Processing
{
    public static class BuildableSmeltery
    {
        public static readonly Quaternion ModelRotation = Quaternion.Euler(0f, 90f, 0f);
        public static readonly Quaternion ScreenRotation = Quaternion.Euler(0f, 90f, 10f);
        public static readonly Quaternion UIRotation = Quaternion.Euler(10f, 0f, 0f);

        public static readonly CraftData.BackgroundType InactiveType = CraftData.BackgroundType.Normal;
        public static readonly CraftData.BackgroundType ActiveType = CraftData.BackgroundType.PlantAirSeed;

        public const int Width = 3;
        public const int Height = 3;
        public const int PreWidth = 6;
        public const int PreHeight = 6;

        public const string InputRoot = nameof(InputRoot);
        public const string OutputRoot = nameof(OutputRoot);
        public const string ChamberRoot = nameof(ChamberRoot);
        public const string PreOutputRoot = nameof(PreOutputRoot);
        public const string InputStorageRoot = nameof(InputStorageRoot);
        public const string OutputStorageRoot = nameof(OutputStorageRoot);
        public const string ChamberStorageRoot = nameof(ChamberStorageRoot);
        public const string PreOutputStorageRoot = nameof(PreOutputStorageRoot);

        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaSmeltery")
                .WithIcon(SpriteManager.Get(TechType.BaseBioReactor));

            CustomPrefab prefab = new(Info);
            BasePieceCloneTemplate template = new(Info, Base.Piece.RoomBioReactorUnderDome);

            template.ModifyPrefab += ModifyPrefab;

            prefab.SetRecipe(new RecipeData()
            {
                Ingredients = new()
                {
                    new Ingredient(TechType.PlasteelIngot, 1),
                    new Ingredient(TechType.Lead, 2),
                    new Ingredient(TechType.EnameledGlass, 2)
                }
            });

            prefab.SetPdaGroupCategory(TechGroup.InteriorPieces, TechCategory.InteriorPiece);

            prefab.SetGameObject(template);
            prefab.Register();
        }

        private static void ModifyPrefab(GameObject prefab)
        {
            prefab.EnsureComponent<StartDelayer>();

            prefab.EnsureComponent<PrefabIdentifier>().classId = Info.ClassID;
            prefab.EnsureComponent<TechTag>().type = Info.TechType;
            prefab.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;

            GameObject.DestroyImmediate(prefab.GetComponent<BaseBioReactorGeometry>());
            GameObject.DestroyImmediate(prefab.GetComponent<BaseDeconstructable>());
            GameObject.DestroyImmediate(prefab.GetComponent<BaseModuleLighting>());
            GameObject.DestroyImmediate(prefab.GetComponentInChildren<TextMeshProUGUI>(true).gameObject);
            GameObject.DestroyImmediate(prefab.GetComponentInChildren<PlayerTriggerAnimation>(true).gameObject);

            Transform screen = prefab.GetComponentInChildren<GenericHandTarget>(true).transform;
            GameObject.DestroyImmediate(screen.GetComponent<GenericHandTarget>());
            GameObject.DestroyImmediate(screen.GetComponent<HighlightingRendererProvider>());

            screen.name = nameof(screen);
            screen.localPosition = new Vector3(screen.localPosition.z, screen.localPosition.y, -screen.localPosition.x);
            screen.localRotation = ScreenRotation;

            CapsuleCollider mainCollider = prefab.GetComponentInChildren<CapsuleCollider>(true);
            GameObject handTarget = prefab.CreateChild(nameof(handTarget));

            CapsuleCollider handTargetCollider = handTarget.EnsureComponent<CapsuleCollider>();
            handTargetCollider.radius = mainCollider.radius;
            handTargetCollider.height = mainCollider.height;

            GameObject.DestroyImmediate(mainCollider.gameObject);

            prefab.transform.Find("collisions").localRotation *= ModelRotation;

            prefab.GetComponents<ConstructableBounds>().ForEach(bounds => bounds.bounds.extents.y = 1.35f);

            Canvas canvas = prefab.GetComponentInChildren<Canvas>(true);
            canvas.gameObject.EnsureComponent<uGUI_GraphicRaycaster>();

            Transform ui = canvas.transform.parent.transform;
            ui.localRotation = UIRotation;
            ui.localPosition = new Vector3(ui.localPosition.z, ui.localPosition.y, -ui.localPosition.x);

            GameObject model = prefab.GetComponentInChildren<Animator>(true).gameObject;
            model.transform.localRotation *= ModelRotation;

            Constructable constructable = PrefabUtils.AddConstructable(prefab, Info.TechType, ConstructableFlags.Rotatable, model);
            constructable.placeDefaultDistance = 5f;

            ModuleSnapper.SetSnappingRules(
                constructable,
                ModuleSnapper.RoomRule.SmallRoom | ModuleSnapper.RoomRule.LargeRoom,
                PlacementRule.SnapToCenter,
                new TransformationRule(SnappedRotationRule.NoOffsetCardinal));

            GameObject input = prefab.CreateChild(InputRoot)
                .WithClassID<ChildObjectIdentifier>(InputRoot)
                .EnsureComponentChained<StorageContainerProvider>(out _);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: input,
                position: Vector3.left * 2f,
                rotation: Quaternion.identity,
                PortType.Input);

            GameObject output = prefab.CreateChild(OutputRoot)
                .WithClassID<ChildObjectIdentifier>(OutputRoot)
                .EnsureComponentChained<StorageContainerProvider>(out _);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: output,
                position: Vector3.right * 2f,
                rotation: Quaternion.identity,
                PortType.Output);

            GameObject chamber = prefab.CreateChild(ChamberRoot)
                .WithClassID<ChildObjectIdentifier>(ChamberRoot)
                .EnsureComponentChained<StorageContainerProvider>(out _);

            GameObject preOutput = prefab.CreateChild(PreOutputRoot)
                .WithClassID<ChildObjectIdentifier>(PreOutputRoot)
                .EnsureComponentChained<StorageContainerProvider>(out _);

            Smeltery smeltery = prefab.EnsureComponent<Smeltery>()
                .WithHandTarget(handTarget.EnsureComponent<GenericHandTarget>())
                .WithInput(PrefabUtils.AddStorageContainer(input, InputStorageRoot, InputStorageRoot, Width, Height))
                .WithOutput(PrefabUtils.AddStorageContainer(output, OutputStorageRoot, OutputStorageRoot, Width, Height))
                .WithChamber(PrefabUtils.AddStorageContainer(chamber, ChamberStorageRoot, ChamberStorageRoot, Width, Height))
                .WithPreOutput(PrefabUtils.AddStorageContainer(preOutput, PreOutputStorageRoot, PreOutputStorageRoot, PreWidth, PreHeight))
                .SetupUI(screen.gameObject);

            Vector2 iconSize = new Vector2(100f, 100f);
            float radius = 20f;

            canvas.WithText(smeltery.heatInfoUIData.SetText)
                    .WithBackgroundIcon(ActiveType, radius, iconSize, smeltery.noHeatUIData.SetBackground)
                    .WithText(smeltery.noHeatUIData.SetText)
                    .WithBackgroundIcon(InactiveType, radius, iconSize, smeltery.lowHeatUIData.SetBackground)
                    .WithText(smeltery.lowHeatUIData.SetText)
                    .WithBackgroundIcon(InactiveType, radius, iconSize, smeltery.medHeatUIData.SetBackground)
                    .WithText(smeltery.medHeatUIData.SetText)
                    .WithBackgroundIcon(InactiveType, radius, iconSize, smeltery.highHeatUIData.SetBackground)
                    .WithText(smeltery.highHeatUIData.SetText);

            prefab.EnsureComponent<PoweredCanvas>().WithCanvas(canvas);
            prefab.EnsureComponent<ConstructableCanvas>().WithCanvas(canvas);

            smeltery.heatInfoUIData.text.gameObject.EnsureComponent<UITranslator>()
                .Setup(smeltery.heatInfoUIData.text, "InfoHeat_IndustricaSmeltery");

            smeltery.noHeatUIData.text.gameObject.EnsureComponent<UITranslator>()
                .Setup(smeltery.noHeatUIData.text, "InfoNoHeat_IndustricaSmeltery");

            smeltery.lowHeatUIData.text.gameObject.EnsureComponent<UITranslator>()
                .Setup(smeltery.lowHeatUIData.text, "InfoLowHeat_IndustricaSmeltery");

            smeltery.medHeatUIData.text.gameObject.EnsureComponent<UITranslator>()
                .Setup(smeltery.medHeatUIData.text, "InfoMediumHeat_IndustricaSmeltery");

            smeltery.highHeatUIData.text.gameObject.EnsureComponent<UITranslator>()
                .Setup(smeltery.highHeatUIData.text, "InfoHighHeat_IndustricaSmeltery");

            Vector2 center = Vector2.one * 0.5f;
            Vector2 up = Vector2.up * 0.3f;
            Vector2 right = Vector2.right * 0.3f;

            smeltery.noHeatUIData.SetAnchor(center + up);
            smeltery.lowHeatUIData.SetAnchor(center + right);
            smeltery.medHeatUIData.SetAnchor(center - up);
            smeltery.highHeatUIData.SetAnchor(center - right);
        }
    }
}
