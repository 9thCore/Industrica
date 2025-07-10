using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Extensions;
using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Item.Network
{
    public static class ItemTransferPipe
    {
        public static PrefabInfo Info { get; private set; }

        private static Atlas.Sprite _icon;
        public static Atlas.Sprite Icon => _icon ??= PathUtil.GetImage("Item/ItemTransportPipe");

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaItemPipe", true)
                .WithIcon(Icon);

            var prefab = new CustomPrefab(Info);
            var obj = new CloneTemplate(Info, TechType.Pipe);

            obj.ModifyPrefab += go =>
            {
                PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Far);
                OxygenPipe pipe = go.GetComponent<OxygenPipe>();
                TransferPipe tool = go.AddComponent<TransferPipe>();

                ComponentUtil.Setup(tool, pipe);
                tool.Setup(pipe);
                tool.type = TransferPipe.PipeType.Item;

                GameObject.DestroyImmediate(pipe.endCap.GetComponent<Collider>());
                go.DestroyImmediateChildrenWith<Collider>(true);
                GameObject.DestroyImmediate(pipe.bottomSection.gameObject);
                GameObject.DestroyImmediate(pipe);
                GameObject.DestroyImmediate(go.GetComponentInChildren<OxygenArea>());
                GameObject.DestroyImmediate(go.GetComponent<FMOD_CustomLoopingEmitter>());
            };

            prefab.SetEquipment(EquipmentType.Hand);
            prefab.SetGameObject(obj);
            prefab.Register();
        }
    }
}
