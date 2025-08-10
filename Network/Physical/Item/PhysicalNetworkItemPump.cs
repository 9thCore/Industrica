using Industrica.Network.Container;
using Industrica.Save;

namespace Industrica.Network.Physical.Item
{
    public class PhysicalNetworkItemPump : PhysicalNetworkPump<Pickupable, PumpContainer<Pickupable>>
    {
        private SaveData save;

        public override PumpContainer<Pickupable> GetContainer => new PumpContainer<Pickupable>(Input, Output);

        public override void CreateSave()
        {
            save = new(this);
        }

        public override void InvalidateSave()
        {
            save.Invalidate();
        }

        public class SaveData : BaseSaveData<SaveData, PhysicalNetworkItemPump>
        {
            public SaveData(PhysicalNetworkItemPump component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.physicalItemPumpSaveData;
        }
    }
}
