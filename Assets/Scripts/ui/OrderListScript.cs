using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using static OrderController;
using TMPro;

public class OrderListScript : MonoBehaviour
{
    public GameObject orderPrefab;
    private List<GameObject> orderItems = new List<GameObject>();
    private OrderController orderController;

    void Start()
    {
        orderController = FindFirstObjectByType<OrderController>();
        Debug.Assert(orderController != null);

        orderController.OnOrderListChange += UpdateOrderList;
    }

    void OnDestroy()
    {

    }

    void UpdateOrderList(List<Order> orders)
    {
        var currentOrders = new List<GameObject>(GameObject.FindGameObjectsWithTag("OrderUIList"));

        foreach (var order in currentOrders) {
            Destroy(order);
        }

        foreach (var order in orders)
        {
            var orderItem = Instantiate(orderPrefab, transform);
            orderItem.transform.SetSiblingIndex(2);

            orderItem.GetComponent<OrderElementScript>().SetClient(order.table.client);
            
            orderItems.Add(orderItem);
        }
    }
}
