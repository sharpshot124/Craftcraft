public class StackableItem : Item
{
    public int maxStackCount = 2;
    public int stackCount = 1;

    public void Combine(StackableItem other)
    {
        if (other.itemName != itemName)
            throw new ItemMismatchException();

        if (other.stackCount + stackCount > maxStackCount)
            throw new StackCapacityException();

        stackCount += other.stackCount;
        other.stackCount = 0;
    }
}
