using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace Industrica.Save
{
    public abstract class AbstractSaveData
    {
        protected void LoadDataLate()
        {
            CoroutineHost.StartCoroutine(LoadLate());
        }

        private IEnumerator LoadLate()
        {
            yield return new WaitForEndOfFrame();
            Load();
        }

        public virtual void TryLoad<S>(Dictionary<string, S> storage, string id) where S : AbstractSaveData
        {
            if (this is not S save)
            {
                Plugin.Logger.LogError($"{this} is not of type {typeof(S)}, cannot load.");
                return;
            }

            if (storage.TryGetValue(id, out S data))
            {
                CopyFromStorage(data);
            }

            storage[id] = save;
        }

        public abstract void Load();
        public abstract void Save();
        public abstract bool ValidForSaving();
        public abstract void CopyFromStorage(AbstractSaveData other);
    }
}
