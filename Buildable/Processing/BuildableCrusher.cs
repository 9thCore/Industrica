using Industrica.ClassBase.Modules.ProcessingMachine;
using Industrica.Machine.Processing;
using Industrica.Network;
using Industrica.Network.Container.Provider.Item;
using Industrica.Network.Pipe.Item;
using Industrica.UI.Inventory.Custom.Info.Text;
using Industrica.UI.Inventory.Custom.Info.Image;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;
using Industrica.UI.Inventory.Custom.Info.ItemIcon;

namespace Industrica.Buildable.Processing
{
    public static class BuildableCrusher
    {
        public const int Width = 3;
        public const int Height = 2;

        public const string InputRoot = nameof(InputRoot);
        public const string OutputRoot = nameof(OutputRoot);
        public const string PreOutputRoot = nameof(PreOutputRoot);
        public const string InputStorageRoot = nameof(InputStorageRoot);
        public const string OutputStorageRoot = nameof(OutputStorageRoot);
        public const string PreOutputStorageRoot = nameof(PreOutputStorageRoot);

        public const float GearRotation = -240f;
        public const float CrushRotationRange = 30f;

        public static readonly Vector2 LeftContainerOffset = new Vector2(-120f, 0f);
        public static readonly Vector2 LeftGearOffset = new Vector2(-37.5f, -240f);
        public static readonly Vector2 LeftDecorativeGearOffset = new Vector2(-81f, -60f);
        public static readonly Vector2 ItemInputOffset = new Vector2(0f, 45f);

        public static readonly Vector2 InputContainerUIPosition = LeftContainerOffset.FromStorageCenter();
        public static readonly Vector2 OutputContainerUIPosition = LeftContainerOffset.MirrorOffsetFromStorageCenter(UIUtil.Axis.Horizontally);

        public static readonly Vector2 LeftGearIconUIPosition = LeftGearOffset.FromStorageCenter();
        public static readonly Vector2 RightGearIconUIPosition = LeftGearOffset.MirrorOffsetFromStorageCenter(UIUtil.Axis.Horizontally);

        public static readonly Vector2 BetweenGearUIPosition = (LeftGearIconUIPosition + RightGearIconUIPosition) / 2f;

        public static readonly Vector2 ItemInputUIPosition = ItemInputOffset.FromArbitraryCenter(BetweenGearUIPosition);
        public static readonly Vector2 ItemOutputUIPosition = ItemInputOffset.MirrorOffsetFromArbitraryCenter(
            BetweenGearUIPosition,
            UIUtil.Axis.Vertically);

        public static readonly Vector2 LeftDecorativeGearIconUIPosition = LeftDecorativeGearOffset.FromArbitraryCenter(BetweenGearUIPosition);
        public static readonly Vector2 RightDecorativeGearIconUIPosition = LeftDecorativeGearOffset.MirrorOffsetFromArbitraryCenter(
            BetweenGearUIPosition,
            UIUtil.Axis.Both);

        public static readonly Vector2 ProcessingUIPosition = new Vector2(0f, 200f).FromStorageCenter();

        public static readonly Vector3 PortCommonOffset = Vector3.up * 1.95f + Vector3.forward * -0.23f;
        public static readonly Vector3 InputPortOffset = Vector3.right * 0.64f;
        public static readonly Vector3 OutputPortOffset = -InputPortOffset;
        public static ProcessPercentageTextInfo ProcessPercentage { get; private set; }
        public static ProcessProgressBarTextureInfo ProcessProgressBar { get; private set; }
        public static ProcessProgressBarTextureInfo ProcessProgressBarHint { get; private set; }
        public static RotatingSpriteInfo LeftGear { get; private set; }
        public static RotatingSpriteInfo LeftDecorativeGear { get; private set; }
        public static RotatingSpriteInfo RightGear { get; private set; }
        public static RotatingSpriteInfo RightDecorativeGear { get; private set; }
        public static IconInfo ProcessingItem { get; private set; }

        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            ProcessPercentage = new()
            {
                Format = "IndustricaProcessPercentageFormat",
                Position = ProcessingUIPosition
            };

            ProcessProgressBar = new()
            {
                Color = Color.white,
                Position = ProcessingUIPosition
            };

            ProcessProgressBarHint = new()
            {
                Color = Color.gray,
                Position = ProcessingUIPosition,
                Percentage = 1f
            };

            LeftGear = new()
            {
                Sprite = UIUtil.SimpleGear,
                Position = LeftGearIconUIPosition,
                rotationSpeed = GearRotation
            };

            RightGear = new()
            {
                Sprite = UIUtil.SimpleGear,
                Position = RightGearIconUIPosition,
                Rotation = 22.5f,
                rotationSpeed = -GearRotation
            };

            LeftDecorativeGear = new()
            {
                Sprite = UIUtil.SimpleGear,
                Position = LeftDecorativeGearIconUIPosition,
                Rotation = -5.625f,
                rotationSpeed = -GearRotation
            };

            RightDecorativeGear = new()
            {
                Sprite = UIUtil.SimpleGear,
                Position = RightDecorativeGearIconUIPosition,
                Rotation = 16.875f,
                rotationSpeed = GearRotation
            };

            ProcessingItem = new()
            {
                Sprite = SpriteManager.defaultSprite
            };

            Info = PrefabInfo
                .WithTechType("IndustricaCrusher")
                .WithIcon(SpriteManager.Get(TechType.VendingMachine));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, TechType.VendingMachine);

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

            prefab.SetPdaGroupCategory(TechGroup.InteriorModules, TechCategory.InteriorModule);

            prefab.SetGameObject(template);
            prefab.Register();
        }

        private static void ModifyPrefab(GameObject prefab)
        {
            prefab.EnsureComponent<DelayedStart>();

            prefab.EnsureComponent<PrefabIdentifier>().classId = Info.ClassID;
            prefab.EnsureComponent<TechTag>().type = Info.TechType;
            prefab.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;

            GameObject.DestroyImmediate(prefab.transform.Find("Vending_machine/vending_machine_snacks").gameObject);
            GameObject.DestroyImmediate(prefab.transform.Find("collisions/handTarget").gameObject);
            GameObject.DestroyImmediate(prefab.GetComponent<VendingMachine>());

            GameObject input = prefab.CreateChild(InputRoot)
                .WithClassID<ChildObjectIdentifier>(InputRoot)
                .EnsureComponentChained<StorageContainerProvider>(out _);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: input,
                position: InputPortOffset + PortCommonOffset,
                rotation: Quaternion.Euler(0f, 0f, 270f),
                PortType.Input);

            GameObject output = prefab.CreateChild(OutputRoot)
                .WithClassID<ChildObjectIdentifier>(OutputRoot)
                .EnsureComponentChained<StorageContainerProvider>(out _);

            TransferItemPort.CreatePort(
                prefab: prefab,
                root: output,
                position: OutputPortOffset + PortCommonOffset,
                rotation: Quaternion.Euler(0f, 0f, 90f),
                PortType.Output);

            GameObject preOutput = prefab.CreateChild(PreOutputRoot)
                .WithClassID<ChildObjectIdentifier>(PreOutputRoot);

            Crusher crusher = prefab.EnsureComponent<Crusher>();
            crusher.SetHandTarget(prefab.EnsureComponent<GenericHandTarget>());

            crusher.BeginInputModuleSetup().WithStorageContainer(
                PrefabUtils.AddStorageContainer(input, InputStorageRoot, InputStorageRoot, Width, Height),
                validHandTarget: false,
                InputContainerUIPosition).SetInteraction(ProcessingMachineModule.Interaction.InputContainer);

            crusher.BeginOutputModuleSetup().WithStorageContainer(
                PrefabUtils.AddStorageContainer(output, OutputStorageRoot, OutputStorageRoot, Width, Height),
                validHandTarget: false,
                OutputContainerUIPosition).SetInteraction(ProcessingMachineModule.Interaction.OutputContainer);

            crusher.BeginPreOutputModuleSetup().WithStorageContainer(
                PrefabUtils.AddStorageContainer(preOutput, PreOutputStorageRoot, PreOutputStorageRoot, Width, Height),
                validHandTarget: false).SetInteraction(ProcessingMachineModule.Interaction.HiddenContainer);
        }
    }
}
