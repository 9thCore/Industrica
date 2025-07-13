using Industrica.Network.Physical;
using Industrica.Save;
using Nautilus.Assets;

namespace Industrica.Item.Network.Placed
{
    public class PlacedItemTransferPipe : PlacedTransferPipe<Pickupable>
    {
        private SaveData save;
        public static PrefabInfo Info { get; private set; }

        public static void Register()
        {
            Info = Register<PlacedItemTransferPipe>("IndustricaPlacedItemTransferPipe");
        }

        protected override void CreateSave()
        {
            save = new(this);
        }

        protected override void InvalidateSave()
        {
            save.Invalidate();
        }

        protected override void OnObjectDestroySave()
        {
            if (save.Valid)
            {
                save.Save();
            }
        }

        public class SaveData : BaseSaveData<SaveData, PlacedItemTransferPipe>
        {
            public SaveData(PlacedItemTransferPipe component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.placedItemTransferPipeData;
        }
    }
}
