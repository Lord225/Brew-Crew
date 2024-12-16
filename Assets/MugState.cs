using Unity.VisualScripting;
using UnityEngine;

public class MugState : MonoBehaviour
{
    [SerializeField]
    public enum State
    {
        Empty,
        Expresso,
        Latte,
        Cappuccino,
        Mocha,
        Americano,
        Macchiato,
        FlatWhite,
    }

    public State state = State.Empty;

    void Start()
    {
       
    }

    void Update()
    {
        
    }
}
