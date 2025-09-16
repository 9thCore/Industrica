using Nautilus.Handlers;
using System.Collections;

namespace Industrica.Recipe
{
    public static class FabricatorRecipes
    {
        //public const string BasicProcessing = "IndustricaBasicProcessing";

        //public static readonly TechCategory BasicProcessingCategory = EnumHandler.AddEntry<TechCategory>(BasicProcessing)
        //    .RegisterToTechGroup(TechGroup.Resources);

        public static IEnumerator Register(WaitScreenHandler.WaitScreenTask task)
        {
            yield break;

            //CraftTreeHandler.AddTabNode(
            //    CraftTree.Type.Fabricator,
            //    BasicProcessing,
            //    string.Empty,
            //    SpriteManager.Get(TechType.LimestoneChunk),
            //    "Resources".AsCraftPath());
        }
    }
}
