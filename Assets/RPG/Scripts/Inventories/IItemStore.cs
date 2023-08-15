namespace GameDevTV.Inventories
{
    public interface IItemStore
    {
        int AddInventoryItems(InventoryItem item, int number);
        int AddAbilityItems(AbilityItem ability, int number);
    }
}
