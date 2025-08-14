using Industrica.Network;
using Industrica.Utility;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using Nautilus.Utility;
using System.Collections;
using UnityEngine;

namespace Industrica.Item.Tool
{
    public static class ItemMultiTool
    {
        public static PrefabInfo Info { get; private set; }

        private static Sprite _icon;
        public static Sprite Icon => _icon ??= SpriteManager.Get(TechType.Builder);

        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaMultiTool", true)
                .WithIcon(Icon);

            var prefab = new CustomPrefab(Info);
            var template = new CloneTemplate(Info, TechType.Builder);

            template.ModifyPrefabAsync += ModifyPrefab;
            
            prefab.SetEquipment(EquipmentType.Hand);
            prefab.SetGameObject(template);
            prefab.Register();
        }

        public static IEnumerator ModifyPrefab(GameObject go)
        {
            GameObject storage = go.CreateChild(nameof(storage));

            CoroutineTask<GameObject> task = CraftData.GetPrefabForTechTypeAsync(TechType.Pipe);
            yield return task;

            PrefabUtils.AddBasicComponents(go, Info.ClassID, Info.TechType, LargeWorldEntity.CellLevel.Medium);
            BuilderTool builder = go.GetComponent<BuilderTool>();
            MultiTool tool = go.AddComponent<MultiTool>();

            ComponentUtil.Setup(tool, builder);
            Object.DestroyImmediate(builder.nozzleLeft.gameObject);
            Object.DestroyImmediate(builder.nozzleRight.gameObject);
            Object.DestroyImmediate(builder);

            OxygenPipe pipe = task.GetResult().GetComponent<OxygenPipe>();
            tool.stretchedPart = UWE.Utils.InstantiateDeactivated(pipe.stretchedPart.gameObject).transform.WithParent(storage.transform).gameObject;
            tool.endCap = UWE.Utils.InstantiateDeactivated(pipe.endCap.gameObject).transform.WithParent(storage.transform).gameObject;

            Object.DestroyImmediate(tool.stretchedPart.GetComponent<Collider>());
            Object.DestroyImmediate(tool.stretchedPart.GetComponentInChildren<Collider>(true).gameObject);
            Object.DestroyImmediate(tool.endCap.GetComponent<Collider>());
            Object.DestroyImmediate(tool.endCap.GetComponentInChildren<Collider>(true).gameObject);

            tool.Setup(builder.bar.materials[builder.barMaterialID]);
        }
    }
}
