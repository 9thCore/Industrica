using Industrica.Item.Generic.Builder;
using System;

namespace Industrica.Item.Generic.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public abstract class AbstractItemAttribute : Attribute
    {
        public abstract AbstractItemBuilder GetBuilder(string classID);
    }
}
