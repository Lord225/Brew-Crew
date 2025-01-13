using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class ClientScript : MonoBehaviour
{
    public enum State
    {
        Spawned,
        GoingToTable,
        MakingOrder,
        WaitingForOrder,
        Eating,
        Leaving,
        Wardering,
    }

    public OrderController.Order order;
    public State currentState;
    public TableScript targetTable;
    private OrderController orderController;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private IconScript icon;

    public Color beginColor = Color.white;
    public Color endColor = Color.red;
    
    public delegate void OnStateChange(State state);

    public OnStateChange OnStateChangeHandler;


    void Start()
    {
        currentState = State.Spawned;
        orderController = FindFirstObjectByType<OrderController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        icon = GetComponent<IconScript>();

        Debug.Assert(orderController != null);
        Debug.Assert(navMeshAgent != null);
        Debug.Assert(animator != null);
        Debug.Assert(icon != null);

        // set layers used by navMeshAgent ()

        FindClosestEmptyTable();
    }

    void resetAnimations()
    {
        animator.SetBool("waiting", false);
        animator.SetBool("waitingForSeat", false);
        animator.SetBool("eating", false);
    }

    void Update()
    {
        resetAnimations();

        switch (currentState)
        {
            case State.Spawned:
                GoToTable();
                break;
            case State.GoingToTable:
                GoingToTable();
                break;
            case State.MakingOrder:
                MakeOrder();
                break;
            case State.WaitingForOrder:
                WaitingForOrder();
                animator.SetBool("waiting", true);
                break;
            case State.Eating:
                animator.SetBool("eating", true);
                Eat();
                break;
            case State.Leaving:
                Leave();
                break;
            case State.Wardering:
                animator.SetBool("waitingForSeat", true);
                Wardering();
                break;
        }
    }

    void WaitingForOrder() {
        var myorder = orderController.GetOrderForTable(targetTable);

        var limit = myorder.minimumServeTime;
        var orderTime = myorder.orderTime;
        var time = orderController.elapsedTime;

        var lerp = Mathf.InverseLerp(orderTime, limit, time);
    
        icon.color = Color.Lerp(beginColor, endColor, lerp);

        if (time > limit)
        {
            Debug.Log("Client waited too long for the order");
            SetStateToLeave();
            OnStateChangeHandler?.Invoke(currentState);
        }
    }


    private void Wardering()
    {
        if (navMeshAgent.remainingDistance < 3f)
        {
            if (Random.Range(0, 100) < 10)
            {
                SetStateToLeave();
                return;
            }

            var randomDirection = Random.insideUnitSphere * 10;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, 10, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        } 
    }

    private void GoingToTable()
    {
        if (navMeshAgent.remainingDistance < 1.5f)
        {
            currentState = State.MakingOrder;
            OnStateChangeHandler?.Invoke(currentState);
        }
    }

    private void FindClosestEmptyTable()
    {
        GameObject[] tables = GameObject.FindGameObjectsWithTag("Table");

        if(targetTable != null)
            targetTable.SetEmpty(true);


        targetTable = tables
            .Select(table => table.GetComponent<TableScript>())
            .Where(tableScript => tableScript != null && tableScript.IsEmpty)
            .OrderBy(tableScript => Vector3.Distance(transform.position, tableScript.transform.position) + Random.Range(0f, 1f))
            .FirstOrDefault();

        if (targetTable != null) {
            order.table = targetTable;
            targetTable.client = this;
            targetTable.SetEmpty(false);
        }
        else  {
            currentState = State.Wardering;
            OnStateChangeHandler?.Invoke(currentState);
        }
    }

    private void FindClosestDoor()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        var closestDoor = doors
            .Select(door => door.GetComponentInChildren<DoorScript>())
            .Where(door => door != null)
            .OrderBy(door => Vector3.Distance(transform.position, door.transform.position) + Random.Range(0f, 1f))
            .FirstOrDefault();

        if (closestDoor != null)
        {
            navMeshAgent.SetDestination(closestDoor.spawn.position);
            navMeshAgent.stoppingDistance = 1.5f;
        } else {
            Destroy(gameObject);
        }
    }

    private void GoToTable()
    {
        if (targetTable != null)
        {
            navMeshAgent.SetDestination(targetTable.transform.position);
            navMeshAgent.stoppingDistance = 1.5f;
            currentState = State.GoingToTable;
            OnStateChangeHandler?.Invoke(currentState);
        } else {
            FindClosestEmptyTable();
        }
    }

    private void MakeOrder()
    {
        if (targetTable != null)
        {
            Debug.Log("Making order");
            orderController.AddOrder(targetTable);
            targetTable.client = this;
            currentState = State.WaitingForOrder;
            icon.description = MugState.StateToString(order.requestedMug);
            OnStateChangeHandler?.Invoke(currentState);
        }
    }

    private void Eat()
    {
        StartCoroutine(EatAndLeave());
    }

    private IEnumerator EatAndLeave()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        SetStateToLeave();
    }

    private void Leave()
    {
        if (navMeshAgent.remainingDistance < 0.1f){
            Destroy(gameObject);
        }
    }

    public void SetStateToEat()
    {
        Debug.Log("Client got the order");
        currentState = State.Eating;
        OnStateChangeHandler?.Invoke(currentState);
    }

    public void SetStateToLeave()
    {
        FindClosestDoor();

        currentState = State.Leaving;
        OnStateChangeHandler?.Invoke(currentState);
        // set destination to the door

        // set the table to empty
        if (targetTable != null)
        {
            targetTable.SetEmpty(true);
            targetTable.client = null;
        }
    }
}
