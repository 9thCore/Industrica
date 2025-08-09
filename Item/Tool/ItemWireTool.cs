using Industrica.Network.Wire;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using UnityEngine;

namespace Industrica.Item.Tool
{
    public class ItemWireTool
    {
        public static PrefabInfo Info { get; private set; }

        private static Atlas.Sprite _icon;
        public static Atlas.Sprite Icon => _icon ??= PathUtil.GetImage("Item/ItemTransportPipe");

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaWire", true)
                .WithIcon(Icon);

            var prefab = new CustomPrefab(Info);
            var obj = new CloneTemplate(Info, TechType.Pipe);

            obj.ModifyPrefab += go =>
            {
                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Far);
                OxygenPipe pipe = go.GetComponent<OxygenPipe>();
                WireTool tool = go.AddComponent<WireTool>();

                ComponentUtil.Setup(tool, pipe);
                tool.Setup(pipe);

                Object.DestroyImmediate(pipe.endCap.GetComponent<Collider>());
                go.DestroyImmediateChildrenWith<Collider>(true);
                Object.DestroyImmediate(pipe.bottomSection.gameObject);
                Object.DestroyImmediate(pipe);
                Object.DestroyImmediate(go.GetComponentInChildren<OxygenArea>());
                Object.DestroyImmediate(go.GetComponent<FMOD_CustomLoopingEmitter>());
            };

            prefab.SetEquipment(EquipmentType.Hand);
            prefab.SetGameObject(obj);
            prefab.Register();
        }
    }
}
