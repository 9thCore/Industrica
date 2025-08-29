using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;

namespace Industrica.Item.Mining.OreVein
{
    public static class ItemOreVeinResourceTitaniumCopper
    {
        public static PrefabInfo Info { get; private set; }
        public static void Register()
        {
            Info = PrefabInfo
                .WithTechType("IndustricaOreVeinResource_TitaniumCopper")
                .WithIcon(SpriteManager.Get(TechType.LimestoneChunk));

            var prefab = new CustomPrefab(Info);
            var template = new CloneTemplate(Info, TechType.LimestoneChunk);

            template.ModifyPrefab += ItemOreVeinResourceEmpty.ModifyBreakableChunkPrefab(Info);

            prefab.SetGameObject(template);
            prefab.Register();
        }        
    }
}
