using Industrica.Save;
using Nautilus.Assets;
using UnityEngine;

namespace Industrica.Network.Physical.Item
{
    public class PlacedItemTransferPipe : PlacedTransferPipe<Pickupable>
    {
        private SaveData save;
        public static PrefabInfo Info { get; private set; }

        public override Vector3 Scale => Vector3.one;
        public override Color StretchedPartColor => ItemTransferPipe.ItemPipeColor;
        public override Color BendColor => ItemTransferPipe.ItemPipeBendColor;

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

        public class SaveData : TransferPipeSaveData<SaveData, PlacedItemTransferPipe>
        {
            public SaveData(PlacedItemTransferPipe component) : base(component) { }
            public override SaveSystem.SaveData<SaveData> SaveStorage => SaveSystem.Instance.placedItemTransferPipeData;
        }
    }
}
