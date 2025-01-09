using UnityEngine;


[CreateAssetMenu(fileName = "CraftingRecipe", menuName = "Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{  
    
    public class Result {
        public MugState.State state;
        public GameObject item;
    }

    [Header("Item on Table")]
    public MugState.State itemState1; //Item on table
    public PickableItem itemType1;

    [Header("Item in Inventory")]
    public MugState.State itemState2;
    public PickableItem itemType2;

    [Header("Result")]
    public MugState.State resultState;

    public GameObject resultItem;


    public bool Matches(GameObject item1, GameObject item2)
    {
        if(item1 == null)
            return false;
        if(item2 == null)
            return false;

        var item1State = item1.GetComponent<PickableItem>().GetType();
        var item2State = item2.GetComponent<PickableItem>().GetType();

        if (item1State == null || item2State == null)
            return false;

        if(item1State != itemType1.GetType()) 
            return false;

        if(item2State != itemType2.GetType()) 
            return false;
        
        if(item1.TryGetComponent(out MugState state)) 
            if(state.state != itemState1) 
                return false;
            
        if(item2.TryGetComponent(out MugState state2)) 
            if(state2.state != itemState2) 
                return false;
            

        return true;
    }

    public Result getResult() {
        return new Result {
            state = resultState,
            item = resultItem
        };
    }
}