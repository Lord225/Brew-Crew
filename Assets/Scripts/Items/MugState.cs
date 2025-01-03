using Unity.VisualScripting;
using UnityEngine;

public class MugState : PickableItem
{

    private Renderer renderer;
           
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

    [SerializeField]
    private State _state = State.Empty;

    public State state
    {
        get { return _state; }
        set
        {
            _state = value;
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        if (renderer == null)
        {
            // get children named inside
            var child = transform.Find("inside");
            if (child != null)
            {
                renderer = child.GetComponent<Renderer>();
            }
        }

        if (renderer != null)
        {
            switch (_state)
            {
                case State.Empty:
                    renderer.material.color = Color.white;
                    break;
                case State.Expresso:
                    renderer.material.color = new Color(0.4f, 0.2f, 0.1f); // Brown
                    break;
                case State.DoubleExpresso:
                    renderer.material.color = new Color(0.3f, 0.15f, 0.05f); // Darker Brown
                    break;
                case State.Latte:
                    renderer.material.color = new Color(0.9f, 0.8f, 0.7f); // Light Brown
                    break;
                case State.Cappuccino:
                    renderer.material.color = new Color(0.8f, 0.7f, 0.6f); // Medium Brown
                    break;
                case State.Mocha:
                    renderer.material.color = new Color(0.5f, 0.3f, 0.2f); // Chocolate Brown
                    break;
                case State.Americano:
                    renderer.material.color = new Color(0.2f, 0.1f, 0.05f); // Dark Brown
                    break;
                case State.Macchiato:
                    renderer.material.color = new Color(0.7f, 0.5f, 0.3f); // Caramel Brown
                    break;
                case State.FlatWhite:
                    renderer.material.color = new Color(0.95f, 0.9f, 0.85f); // Very Light Brown
                    break;
                case State.Water:
                    renderer.material.color = Color.blue;
                    break;
            }
        }
    }

    void Start()
    {
        UpdateColor();
    }

    void OnValidate()
    {
        UpdateColor();
    }

    void Update()
    {
        
    }
}
