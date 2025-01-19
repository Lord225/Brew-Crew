using TMPro;
using UnityEngine;

public class PointsScript : MonoBehaviour
{
    OrderController orderController;
    TextMeshProUGUI p;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        orderController = GameObject.Find("OrderController").GetComponent<OrderController>();

        p = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        var points = orderController.totalPoints;

        p.text = "Points: " + points * 100;
    }
}
