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

    public MugState.State requestedMug;
    public State currentState;
    public TableScript targetTable;
    private OrderController orderController;
    private NavMeshAgent navMeshAgent;
    private Animator animator;

    private IconScript icon;

    public Color beginColor = Color.white;
    public Color endColor = Color.red;

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
    }


    private void Wardering()
    {
        // Logic for wardering
        // go into random directions

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

        if (targetTable != null)
            targetTable.SetEmpty(false);
        else 
            currentState = State.Wardering;
    }

    private void GoToTable()
    {
        if (targetTable != null)
        {
            navMeshAgent.SetDestination(targetTable.transform.position);
            currentState = State.GoingToTable;
        } else {
            FindClosestEmptyTable();
        }
    }

    private void MakeOrder()
    {
        if (targetTable != null)
        {
            Debug.Log("Making order");
            requestedMug = orderController.AddOrder(targetTable);
            targetTable.client = this;
            currentState = State.WaitingForOrder;
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
        // Logic for leaving
        // if reached the door, destroy
        if (navMeshAgent.remainingDistance < 1.5f)
        {
            Destroy(gameObject);
        }
    }

    public void SetStateToEat()
    {
        Debug.Log("Client got the order");
        currentState = State.Eating;
    }

    public void SetStateToLeave()
    {
        currentState = State.Leaving;

        // set the table to empty
        if (targetTable != null)
        {
            targetTable.SetEmpty(true);
            targetTable.client = null;
        }
    }
}
