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

        var limit = myorder.waitTime; // for how long client can wait for the order
        var orderTime = myorder.orderTime; // when the order was made
        var time = orderController.elapsedTime; // current time

        var lerp = Mathf.InverseLerp(orderTime, orderTime + limit, time);
        icon.color = Color.Lerp(beginColor, endColor, lerp);

        var timePassed = time - orderTime;

        if (timePassed > limit)
        {
            Debug.Log("Client waited too long for the order (" + order.requestedMug + ") time " + time + " orderTime " + orderTime + " limit " + limit); 
            SetStateToLeave();
        }
    }


    private void Wardering()
    {
        navMeshAgent.stoppingDistance = 0.05f;
        if (navMeshAgent.remainingDistance < 0.1f)
        {
            if (Random.Range(0, 100) < 10)
            {
                SetStateToLeave();
                return;
            }
             FindClosestEmptyTable();

             
            if (targetTable != null)
            {
                navMeshAgent.SetDestination(targetTable.transform.position);
                navMeshAgent.stoppingDistance = 1.7f;
                currentState = State.GoingToTable;
                OnStateChangeHandler?.Invoke(currentState);
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
        if (navMeshAgent.remainingDistance < 2.0f)
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

        targetTable = null;

        targetTable = tables
            .Select(table => table.GetComponent<TableScript>())
            .Where(tableScript => tableScript != null && tableScript.IsEmpty)
            .OrderBy(tableScript => Vector3.Distance(transform.position, tableScript.transform.position) + Random.Range(0f, 1f))
            .FirstOrDefault();

        if (targetTable != null) {
            order.table = targetTable;
            targetTable.SetEmpty(false);
            targetTable.client = this;
        }
        else  if (currentState != State.Wardering) {
            currentState = State.Wardering;
            OnStateChangeHandler?.Invoke(currentState);
        }
    }

    private DoorScript FindClosestDoor()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        return doors
            .Select(door => door.GetComponentInChildren<DoorScript>())
            .Where(door => door != null)
            .OrderBy(door => Vector3.Distance(transform.position, door.transform.position) + Random.Range(0f, 1f))
            .FirstOrDefault();
    }

    private void GoToTable()
    {
        if (targetTable != null)
        {
            navMeshAgent.SetDestination(targetTable.transform.position);
            navMeshAgent.stoppingDistance = 1.7f;
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
            var updatedOrder = orderController.AddOrder(targetTable);
            order = updatedOrder;
            Debug.Log("Making order for table " + targetTable.name + " order time: " + order.orderTime + " wait time: " + order.waitTime + " requested mug: " + order.requestedMug);
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
        if (navMeshAgent.remainingDistance < 0.5f){
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
        // set the table to empty
        if (targetTable != null)
        {
            orderController.RemoveOrder(targetTable);  
            targetTable.SetEmpty(true);
            targetTable = null;
        }

        var door = FindClosestDoor();

        if (door != null)
        {
            navMeshAgent.SetDestination(door.spawn.position);
            navMeshAgent.stoppingDistance = 0.05f;

        
            currentState = State.Leaving;
            OnStateChangeHandler?.Invoke(currentState);
        } 
    }
}
