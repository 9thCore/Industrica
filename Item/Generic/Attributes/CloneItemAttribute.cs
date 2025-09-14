using Industrica.Item.Generic.Builder;
using System;

namespace Industrica.Item.Generic.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class CloneItemAttribute : AbstractItemAttribute
    {
        public readonly TechType originalTechType;
        public readonly string originalClassID;
        public readonly LargeWorldEntity.CellLevel cellLevel;

        public CloneItemAttribute(TechType originalTechType, LargeWorldEntity.CellLevel cellLevel)
        {
            this.originalTechType = originalTechType;
            this.cellLevel = cellLevel;
        }

        public CloneItemAttribute(string originalClassID, LargeWorldEntity.CellLevel cellLevel)
        {
            this.originalClassID = originalClassID;
            this.cellLevel = cellLevel;
        }

        public override AbstractItemBuilder GetBuilder(string classID)
        {
            if (string.IsNullOrEmpty(originalClassID))
            {
                return new CloneItemBuilder(classID, originalTechType, cellLevel);
            }

            return new CloneItemBuilder(classID, originalClassID, cellLevel);
        }
    }
}
