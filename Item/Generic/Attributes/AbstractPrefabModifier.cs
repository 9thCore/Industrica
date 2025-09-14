using Industrica.Item.Generic.Builder;
using System;

namespace Industrica.Item.Generic.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class AbstractPrefabModifierAttribute : Attribute
    {
        public abstract void Apply(CloneItemBuilder cloneItemBuilder);
    }
}
