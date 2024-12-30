using Unity.VisualScripting;
using UnityEngine;

public class MugState : PickableItem
{
    public MugState(State state)
    {
        this.state = state;
    }

    [SerializeField]
    public enum State
    {
        Empty, 
        Expresso, 
        DoubleExpresso, 
        Latte, 
        Cappuccino, 
        Mocha, 
        Americano, 
        Macchiato, 
        FlatWhite,
        Water, 
    }

    public State state = State.Empty;

    void Start()
    {
       
    }

    void Update()
    {
        
    }
}
