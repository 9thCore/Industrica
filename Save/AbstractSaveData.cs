using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UWE;

namespace Industrica.Save
{
    public abstract class AbstractSaveData<S> where S : AbstractSaveData<S>
    {
        [JsonIgnore]
        public bool Valid { get; private set; } = true;
        [JsonIgnore]
        public abstract string SaveKey { get; }
        [JsonIgnore]
        public abstract SaveSystem.SaveData<S> SaveStorage { get; }

        public void Invalidate()
        {
            Valid = false;
        }

        protected void LoadDataLate()
        {
            CoroutineHost.StartCoroutine(LoadLate());
        }

        private IEnumerator LoadLate()
        {
            yield return new WaitForEndOfFrame();

            if (this is not S save)
            {
                Plugin.Logger.LogError($"Could not load data, as {this} is not of type {typeof(S).Name}");
                yield break;
            }

            if (!SaveStorage.TryLoad(save))
            {
                yield break;
            }

            Load();
        }

        public virtual void InvalidateIfNotValid()
        {

        }

        public virtual bool IncludeInSave()
        {
            return true;
        }
        
        public void UpdateSaveIfAble()
        {
            if (!AbleToUpdateSave())
            {
                return;
            }

            Save();
        }

        public abstract void Load();
        public abstract void Save();
        public abstract bool AbleToUpdateSave();
        public abstract void CopyFromStorage(S data);
    }
}
