using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Industrica.Save
{
    public abstract class ComponentSaveData<T> : PrefabIdentifiableSaveData where T : MonoBehaviour
    {
        public override PrefabIdentifier GetIdentifier => Component.GetComponent<PrefabIdentifier>();

        [JsonIgnore]
        public T Component { get; protected set; }

        public ComponentSaveData(T component)
        {
            Component = component;

            if (component != null)
            {
                LoadDataLate();
            }
        }

        public override bool ValidForSaving()
        {
            return Component != null;
        }
    }
}
