using Industrica.Save;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Industrica.Operation
{
    public class OperationWrapper
    {
        private static readonly List<Operation> Operations = new();
        private static string defaultOperation;
        public static void Register<T>() where T : Operation, new()
        {
            T operation = new T();

            if (Operations.Count == 0)
            {
                defaultOperation = operation.Id;
            }

            Operations.Add(operation);
        }

        public Operation operation;
        private SaveData save;

        public OperationWrapper()
        {
            if (!Operations.Any())
            {
                return;
            }

            operation = Operations.First();
        }

        public void CreateSave(string id)
        {
            save = new(this, id);
        }

        public void InvalidateSave()
        {
            save.Invalidate();
        }

        public int Apply(int a, int b)
        {
            return operation?.Apply(a, b) ?? default;
        }

        public void CycleOperation()
        {
            if (operation == null
                || !Operations.Any())
            {
                return;
            }

            int index = Operations.IndexOf(operation);
            index = (index + 1) % Operations.Count;
            Set(Operations[index]);
        }

        public void Set(string type)
        {
            if (!Operations.Any())
            {
                return;
            }

            IEnumerable<Operation> match = Operations.Where(c => c.Id == type);
            if (!match.Any())
            {
                match = Operations;
            }

            Set(match.First());
        }

        public void Set(Operation operation)
        {
            this.operation = operation;
            OnChange?.Invoke();
        }

        public delegate void OperationUpdate();
        public OperationUpdate OnChange;

        public class SaveData : AbstractSaveData<SaveData>
        {
            [JsonIgnore]
            private readonly OperationWrapper wrapper;
            [JsonIgnore]
            private readonly string id;

            public string operation;

            public override string SaveKey => id;
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.operationWrapperSaveData;

            public SaveData(OperationWrapper wrapper, string id)
            {
                this.wrapper = wrapper;
                this.id = id;

                if (wrapper != null
                    && id != null)
                {
                    LoadDataLate();
                }
            }

            public override bool AbleToUpdateSave()
            {
                return wrapper != null;
            }

            public override void CopyFromStorage(SaveData data)
            {
                operation = data.operation;
            }

            public override void Load()
            {
                wrapper.Set(operation);
            }

            public override void Save()
            {
                operation = wrapper.operation.Id;
            }

            public override bool IncludeInSave()
            {
                return operation != defaultOperation;
            }
        }
    }
}
