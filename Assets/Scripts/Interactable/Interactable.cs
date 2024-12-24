using UnityEngine;

public class Interactable : MonoBehaviour {
    public virtual bool Interact(Inventory item) {
        return false;
    }
    public virtual bool tryInteract(Inventory item) {
        return false;
    }

    public virtual bool Use() {
        return false;
    }
}

