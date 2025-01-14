using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static OrderController;
using Unity.VisualScripting;
using System.Linq;

public class OrderController : MonoBehaviour
{
    [System.Serializable]
    public class Timing
    {
        public float timestamp;
        public MugState.State mugState;
        public float waitTime;
    }

    public List<Timing> timings = new List<Timing>
    {
        new Timing { timestamp = 10.0f,  mugState = MugState.State.Expresso, waitTime = 60.0f},
        new Timing { timestamp = 40.0f, mugState = MugState.State.Expresso, waitTime = 15.0f },
        new Timing { timestamp = 50.0f, mugState = MugState.State.DoubleExpresso, waitTime = 30.0f },
        
        new Timing { timestamp = 60.0f, mugState = MugState.State.Americano, waitTime = 15.0f },
        new Timing { timestamp = 65.0f,  mugState = MugState.State.Expresso, waitTime = 15.0f},
        new Timing { timestamp = 66.0f,  mugState = MugState.State.Expresso, waitTime = 15.0f},

        new Timing { timestamp = 75.0f,  mugState = MugState.State.Americano, waitTime = 15.0f},
        new Timing { timestamp = 77.0f,  mugState = MugState.State.DoubleExpresso, waitTime = 20.0f},

        new Timing { timestamp = 80.0f, mugState = MugState.State.FlatWhite, waitTime = 45.0f},
        new Timing { timestamp = 85.0f, mugState = MugState.State.FlatWhite, waitTime = 20.0f},
        new Timing { timestamp = 90.0f, mugState = MugState.State.FlatWhite, waitTime = 20.0f},

        new Timing { timestamp = 91.0f, mugState = MugState.State.Expresso, waitTime = 5.0f},
        new Timing { timestamp = 92.0f, mugState = MugState.State.Expresso, waitTime = 5.0f},
    };

    public float elapsedTime = 0.0f;
    public int currentTimingIndex = 0;

    public Order GetOrderForTable(TableScript tableScript) {
        var order = orders.Find(o => o.table == tableScript);

        if(order == null) {
            AddOrder(tableScript);
            return orders.Find(o => o.table == tableScript);
        } else {   
            return order;
        }   
    }

    public void OnGameFinish() {

    }

    void OnValidate()
    {
        var timestamp = 0.0f;
        for(int i = 0; i < timings.Count; i++) {
            var timing = timings[i];
            if (timing.timestamp < timestamp)
            {
                Debug.LogError("Timestamp order constraint not met: Index " + i + " < " + (i - 1));
            }
            else
            {
                timestamp = timing.timestamp;
            }
        }
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (currentTimingIndex >= timings.Count)
        {
            return;
        }

        if (elapsedTime < timings[currentTimingIndex].timestamp)
        {
            return;
        }

        var timing = timings[currentTimingIndex];

        Debug.Log("Spawning client with mug state " + timing.mugState + " at timestamp " + timing.timestamp);


        SpawnClient(timing);

        currentTimingIndex++;

        if (currentTimingIndex >= timings.Count)
        {
            Debug.Log("Game finished");
            OnGameFinish();
        }
    }

    void SpawnClient(Timing timing)
    {
        DoorScript door = GetRandomDoor();
        
        var client = door.SpawnClient();

        if (client != null)
        {
            client.order = new Order
            {
                table = null,
                spawnTime = elapsedTime,
                orderTime = 0.0f,
                waitTime = timing.waitTime,
                requestedMug = timing.mugState
            };
        }
    }

    DoorScript GetRandomDoor()
    {
        // get all game objects with script DoorScript
        var doors = GameObject.FindGameObjectsWithTag("Door");

        // get a random door
        var random = new System.Random();

        return doors[random.Next(doors.Length)].GetComponentInChildren<DoorScript>();
    }


    [System.Serializable]
    public class Order
    {
        public TableScript table = null;
        public float spawnTime = 0.0f;
        public float orderTime = 0.0f;
        public float waitTime = 100.0f;
        public MugState.State requestedMug = MugState.State.Expresso;
    }

    public List<Order> orders = new List<Order>();

    public Order AddOrder(TableScript table)
    {  
        var order = table.client.order;
        order.orderTime = elapsedTime;
        orders.Add(order);
        OnOrderListChange?.Invoke(orders);
        return order;
    }

    public void RemoveOrder(TableScript table)
    {
        var order = orders.Find(o => o.table == table);
        orders.Remove(order);
        OnOrderListChange?.Invoke(orders);
    }

    public delegate void NewOrderEvent(List<Order> orders);

    public event NewOrderEvent OnOrderListChange;
}