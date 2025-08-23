using Industrica.Network.Filter.Holder;
using Industrica.Utility;
using System.Text;

namespace Industrica.UI.Tooltip
{
    public class ItemTechTypeFilterTooltip : AbstractTooltip
    {
        public TechTypeNetworkFilterHolder holder;

        public void WithNetworkFilterHolder(TechTypeNetworkFilterHolder holder)
        {
            this.holder = holder;
        }

        public override void Apply(StringBuilder stringBuilder)
        {
            TooltipFactory.WriteDescription(stringBuilder, "DynamicTooltip_IndustricaItemTechTypeFilter".Translate(holder.TechType.ToString().Translate()));
        }
    }
}
