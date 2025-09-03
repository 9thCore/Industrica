using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;
using System;
using System.Collections;
using UnityEngine;

namespace Industrica.Utility
{
    public class BasePieceCloneTemplate : PrefabTemplate
    {
        public readonly Base.Piece piece;

        public Action<GameObject> ModifyPrefab { get; set; }
        public Func<GameObject, IEnumerator> ModifyPrefabAsync { get; set; }

        public BasePieceCloneTemplate(PrefabInfo info, Base.Piece piece) : base(info)
        {
            this.piece = piece;
        }

        public override IEnumerator GetPrefabAsync(TaskResult<GameObject> gameObject)
        {
            GameObject go = gameObject.Get();
            if(go != null)
            {
                go.SetActive(false);
                yield return ApplyModifications(go);
                yield break;
            }

            TaskResult<Base.PieceDef> definition = new();
            yield return SNUtil.GetBasePieceDefinition(piece, definition);

            GameObject prefab = UWE.Utils.InstantiateDeactivated(definition.Get().prefab.gameObject);
            yield return ApplyModifications(prefab);
            gameObject.Set(prefab);
        }

        private IEnumerator ApplyModifications(GameObject gameObject)
        {
            ModifyPrefab?.Invoke(gameObject);
            if (ModifyPrefabAsync != null)
            {
                yield return ModifyPrefabAsync(gameObject);
            }
        }
    }
}
