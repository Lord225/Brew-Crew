using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static OrderController;

public class OrderController : MonoBehaviour
{
    [System.Serializable]
    public class Difficulty
    {
        public int level;
        public List<MugState.State> states;
        public float minTime;
        public float maxTime;
    }

    public List<Difficulty> difficulties = new List<Difficulty>
    {
        new Difficulty { level = 1, states = new List<MugState.State> { MugState.State.Expresso }, minTime = 5.0f, maxTime = 10.0f },
        new Difficulty { level = 2, states = new List<MugState.State> { MugState.State.Expresso, MugState.State.DoubleExpresso, MugState.State.Americano }, minTime = 5.0f, maxTime = 10.0f },
        new Difficulty { level = 3, states = new List<MugState.State> { MugState.State.Expresso, MugState.State.FlatWhite }, minTime = 5.0f, maxTime = 10.0f },
        new Difficulty { level = 4, states = new List<MugState.State> { MugState.State.Expresso, MugState.State.Latte }, minTime = 5.0f, maxTime = 10.0f },
        new Difficulty { level = 5, states = new List<MugState.State> { MugState.State.Expresso, MugState.State.DoubleExpresso, MugState.State.Americano, MugState.State.Latte, MugState.State.Cappuccino }, minTime = 5.0f, maxTime = 10.0f },
        new Difficulty { level = 6, states = new List<MugState.State> { MugState.State.Expresso, MugState.State.DoubleExpresso, MugState.State.Americano, MugState.State.Latte, MugState.State.Cappuccino, MugState.State.FlatWhite, MugState.State.Mocha }, minTime = 5.0f, maxTime = 10.0f },
        new Difficulty { level = 7, states = new List<MugState.State> { MugState.State.Expresso, MugState.State.DoubleExpresso, MugState.State.Americano, MugState.State.Latte, MugState.State.Cappuccino, MugState.State.FlatWhite, MugState.State.Mocha, MugState.State.Macchiato }, minTime = 5.0f, maxTime = 10.0f }
    };

    [System.Serializable]
    public class DifficultyTiming
    {
        public float timeInSeconds;
        public int peopleToSpawn;
        public int difficultyLevel;
    }

    public List<DifficultyTiming> difficultyTimings = new List<DifficultyTiming>
    {
        new DifficultyTiming { timeInSeconds = 0.0f,  peopleToSpawn = 1, difficultyLevel = 1, },
        new DifficultyTiming { timeInSeconds = 15.0f, peopleToSpawn = 1, difficultyLevel = 1, },
        new DifficultyTiming { timeInSeconds = 20.0f, peopleToSpawn = 2, difficultyLevel = 1,  },
        new DifficultyTiming { timeInSeconds = 30.0f, peopleToSpawn = 1, difficultyLevel = 2,  },
        new DifficultyTiming { timeInSeconds = 31.0f, peopleToSpawn = 1, difficultyLevel = 2,  },
        new DifficultyTiming { timeInSeconds = 32.0f, peopleToSpawn = 1, difficultyLevel = 2,  },
        new DifficultyTiming { timeInSeconds = 40.0f, peopleToSpawn = 1, difficultyLevel = 3,  },
        new DifficultyTiming { timeInSeconds = 45.0f, peopleToSpawn = 1, difficultyLevel = 3,  },
        new DifficultyTiming { timeInSeconds = 50.0f, peopleToSpawn = 1, difficultyLevel = 3,  },
        new DifficultyTiming { timeInSeconds = 360.0f, peopleToSpawn = 1, difficultyLevel = 1,  },
        new DifficultyTiming { timeInSeconds = 420.0f, peopleToSpawn = 1, difficultyLevel = 1,  }
    };
    public float elapsedTime = 0.0f;
    public int currentTimingIndex = 0;
    public int difficulty = 1;


    public Order GetOrderForTable(TableScript tableScript) {
        return orders.Find(o => o.table == tableScript);
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        
        if (currentTimingIndex >= difficultyTimings.Count)
        {
            return;
        }

        if (elapsedTime < difficultyTimings[currentTimingIndex].timeInSeconds)
        {
            return;
        }
        
        var timing = difficultyTimings[currentTimingIndex];
       
        Debug.Log("Spawning " + timing.peopleToSpawn + " people at difficulty " + timing.difficultyLevel);

        difficulty = timing.difficultyLevel;

        SpawnClient();
            
        currentTimingIndex++;
    }

    void SpawnClient()
    {
        DoorScript table = GetRandomDoor();
        
        table.SpawnClient();
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
        public TableScript table;
        public float orderTime;
        public float minimumServeTime;
        public MugState.State requestedMug;
    }

    public Order CreateRandomOrder(TableScript table, int difficulty)
    {
        var difficultyLevel = difficulties.Find(d => d.level == difficulty);
        if (difficultyLevel == null)
        {
            throw new System.ArgumentException("Invalid difficulty level");
        }

        var random = new System.Random();
        var requestedMug = difficultyLevel.states[random.Next(difficultyLevel.states.Count)];

        var order = new Order
        {
            table = table,
            orderTime = Time.time,
            requestedMug = requestedMug,
            minimumServeTime = difficultyLevel.minTime
        };

        return order;
    }

    public List<Order> orders = new List<Order>();

    public MugState.State AddOrder(TableScript table)
    {
        var order = CreateRandomOrder(table, 1);
        orders.Add(order);
        return order.requestedMug;
    }
}