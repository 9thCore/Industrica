using Industrica.Item.Generic.Builder;
using Industrica.Utility;

namespace Industrica.Item.Generic.Attributes
{
    public class ItemBreakableChunkPrefabModifierAttribute : AbstractPrefabModifierAttribute
    {
        public override void Apply(CloneItemBuilder cloneItemBuilder)
        {
            cloneItemBuilder.ModifyPrefab(PrefabUtil.MakeBreakableChunkIntoItem);
        }
    }
}
