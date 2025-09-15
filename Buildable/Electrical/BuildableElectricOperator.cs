using Industrica.Network.Wire;
using Industrica.UI;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Crafting;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Buildable.Electrical
{
    public class BuildableElectricOperator
    {
        public static readonly Texture Texture = PathUtil.GetTexture("ElectricOperator/monitor");

        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaElectricOperator", true)
                .WithIcon(SpriteManager.Get(TechType.ComputerChip));

            CustomPrefab prefab = new(Info);
            CloneTemplate template = new(Info, "0782292e-d313-468a-8816-2adba65bfba3");

            template.ModifyPrefab += (GameObject obj) =>
            {
                obj.EnsureComponent<DelayedStart>();

                Renderer renderer = obj.GetComponentInChildren<Renderer>();

                PrefabUtils.AddBasicComponents(obj, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Global);
                PrefabUtils.AddConstructable(obj, Info.TechType, ConstructableFlags.Inside | ConstructableFlags.Wall, renderer.gameObject);

                Vector3 offset = -Vector3.up * 0.25f;
                obj.SetupConstructableBounds(offset: offset);

                foreach (Transform child in obj.transform)
                {
                    child.position += offset;
                }

                renderer.transform.position += Vector3.right * 0.015f;

                Vector3 inputOffset = Vector3.up * 0.07f;
                Vector3 inputSide = Vector3.right * 0.4f;
                Vector3 forward = Vector3.forward * 0.05f;

                WirePort input1 = WirePort.CreatePort(
                    obj,
                    inputOffset + inputSide + forward,
                    Quaternion.Euler(0f, 0f, 270f),
                    Network.PortType.Input);

                WirePort input2 = WirePort.CreatePort(
                    obj,
                    inputOffset - inputSide + forward,
                    Quaternion.Euler(0f, 0f, 90f),
                    Network.PortType.Input);

                WirePort output = WirePort.CreatePort(
                    obj,
                    offset + forward,
                    Quaternion.Euler(0f, 0f, 180f),
                    Network.PortType.Output);

                renderer.materials[0].SetFloat("_LightmapStrength", 1f);
                renderer.materials[1].SetTexture("_Illum", Texture);
                renderer.materials[1].SetTexture("_SpecTex", Texture);
                renderer.materials[1].mainTexture = Texture;

                WireLogicGate gate = obj.EnsureComponent<WireLogicGate>().WithTarget(obj.EnsureComponent<GenericHandTarget>());
                gate.SetPorts(input1, input2, output);

                CraftData.BackgroundType type = CraftData.BackgroundType.Normal;
                Vector2 iconSize = Vector2.one * 50f;
                float radius = 15f;

                obj.EnsureComponent<PoweredCanvas>().WithCanvas(
                    obj.CreateCanvas()
                    .WithLocalRotation(Quaternion.Euler(0f, 180f, 0f))
                    .WithLocalPosition(Vector3.forward * 0.08f)
                    .WithBackgroundIcon(type, radius, iconSize, gate.input1UIData.SetBackground)
                    .WithText(gate.input1UIData.SetText)
                    .WithBackgroundIcon(type, radius, iconSize, gate.input2UIData.SetBackground)
                    .WithText(gate.input2UIData.SetText)
                    .WithBackgroundIcon(type, radius, iconSize, gate.outputUIData.SetBackground)
                    .WithText(gate.outputUIData.SetText)
                    .WithBackgroundIcon(type, radius, iconSize, gate.operationUI.SetBackground)
                    .WithText(gate.operationUI.SetText));

                Vector2 input2DOffset = Vector2.up * 40f;
                Vector2 input2DSide = Vector2.right * -140f;

                gate.input1UIData.MoveUIData(input2DOffset + input2DSide);
                gate.input2UIData.MoveUIData(input2DOffset - input2DSide);
                gate.outputUIData.MoveUIData(-input2DOffset);
                gate.operationUI.MoveUIData(input2DOffset);
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
