using Newtonsoft.Json;
using UnityEngine;

namespace Industrica.Save
{
    public abstract class ComponentSaveData<S, T> : IdentifiableSaveData<S> where S : AbstractSaveData<S> where T : MonoBehaviour
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

        public override bool AbleToUpdateSave()
        {
            return Valid && Component != null;
        }
    }
}
