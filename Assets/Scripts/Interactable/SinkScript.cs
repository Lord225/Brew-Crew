using UnityEngine;

public class SinkScript : Interactable
{
    public override bool tryInteract(Inventory otherItem)
    {
        if(otherItem.TryGetInventoryItemComponent(out MugState item)) {
            return true;
        }
        return false;
    }

    public override bool Interact(Inventory otherItem)
    {
        if (otherItem.TryGetInventoryItemComponent(out MugState item))
        {
            item.state = MugState.State.Water;
            return true;
        }
        return false;
    }
}
