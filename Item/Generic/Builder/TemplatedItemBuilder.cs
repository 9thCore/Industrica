using Nautilus.Assets;
using Nautilus.Assets.PrefabTemplates;

namespace Industrica.Item.Generic.Builder
{
    public abstract class TemplatedItemBuilder : AbstractItemBuilder
    {
        public TemplatedItemBuilder(string classID) : base(classID) { }

        protected abstract PrefabTemplate CreateTemplate(in PrefabInfo info);

        public override void Build(out PrefabInfo info)
        {
            GetPrefab(out info, out CustomPrefab prefab);
            PrefabTemplate template = CreateTemplate(info);
            prefab.SetGameObject(template);
            prefab.Register();
        }
    }
}
