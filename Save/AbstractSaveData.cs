using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UWE;

namespace Industrica.Save
{
    public abstract class AbstractSaveData
    {
        public abstract string SaveKey { get; }

        protected void LoadDataLate()
        {
            CoroutineHost.StartCoroutine(LoadLate());
        }

        private IEnumerator LoadLate()
        {
            yield return new WaitForEndOfFrame();
            Load();
        }

        public virtual bool TryLoad<S>(Dictionary<string, S> storage) where S : AbstractSaveData
        {
            return TryLoad(storage, SaveKey);
        }

        public virtual bool TryLoad<S>(Dictionary<string, S> storage, string id) where S : AbstractSaveData
        {
            if (this is not S save)
            {
                Plugin.Logger.LogError($"{this} is not of type {typeof(S)}, cannot load.");
                return false;
            }

            bool flag = false;
            if (storage.TryGetValue(id, out S data))
            {
                CopyFromStorage(data);
                flag = true;
            }

            storage[id] = save;
            return flag;
        }

        public abstract void Load();
        public abstract void Save();
        public abstract bool ValidForSaving();
        public abstract void CopyFromStorage(AbstractSaveData other);
    }
}
