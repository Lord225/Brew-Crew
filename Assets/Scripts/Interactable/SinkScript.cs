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
            if(item.state == MugState.State.Empty)
                item.state = MugState.State.Water;
            else if(item.state == MugState.State.Expresso)
                item.state = MugState.State.Americano;
            else if (item.state == MugState.State.Dity)
                item.state = MugState.State.Empty;
            return true;
        }
        return false;
    }
}
