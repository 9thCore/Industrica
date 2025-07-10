using Newtonsoft.Json;
using UnityEngine;

namespace Industrica.Save
{
    public abstract class ComponentSaveData<T> : IdentifiableSaveData where T : MonoBehaviour
    {
        public override UniqueIdentifier GetUniqueIdentifier => Component.GetComponent<UniqueIdentifier>();

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
