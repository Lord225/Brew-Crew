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
            orderItem.transform.SetSiblingIndex(1);
            orderItem.GetComponentInChildren<TextMeshProUGUI>().text = MugState.StateToString(order.requestedMug);
            
            var hourglassIcon = orderItem.transform.Find("hourglass");
            var ok = orderItem.transform.Find("Ok");

            Debug.Assert(hourglassIcon != null);
            Debug.Assert(ok != null);


            if(order.table.client != null) {
                if(order.table.client.isWaitingForOrder()) {
                    hourglassIcon.gameObject.SetActive(false);
                    ok.gameObject.SetActive(true);
                } else if(order.table.client.isEating()) {
                    hourglassIcon.gameObject.SetActive(true);
                    ok.gameObject.SetActive(false);
                } else {
                    hourglassIcon.gameObject.SetActive(false);
                    ok.gameObject.SetActive(false);
                }
            } 

            orderItems.Add(orderItem);
        }
    }
}
