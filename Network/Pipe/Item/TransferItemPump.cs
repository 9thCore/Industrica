using Industrica.Save;

namespace Industrica.Network.Pipe.Item
{
    public class TransferItemPump : TransferPump<Pickupable>
    {
        private SaveData save;

        public override void CreateSave()
        {
            save = new(this);
        }

        public override void InvalidateSave()
        {
            save.Invalidate();
        }

        public class SaveData : BaseSaveData<SaveData, TransferItemPump>
        {
            public SaveData(TransferItemPump component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.physicalItemPumpSaveData;
        }
    }
}
