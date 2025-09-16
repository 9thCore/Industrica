namespace Industrica.Recipe.ExtendedRecipe
{
    public record ItemIngredient(TechType TechType, int Amount = 1) : ExtendedIngredient<InventoryItem>(TechType, Amount)
    {
        public override bool Test(InventoryItem inventoryItem)
        {
            return TechType == inventoryItem.techType;
        }
    }
}
