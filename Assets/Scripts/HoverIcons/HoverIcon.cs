using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HoverIcon : MonoBehaviour
{
    public Image iconImage;
    public Vector3 offset;
    private Camera mainCamera;
    public bool isVisible = false;

    void Start()
    {
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();
        iconImage.enabled = false;
    }

    void Update()
    {
        if (isVisible)
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(transform.position + offset);
            iconImage.transform.position = screenPosition;
        }
    }

    public void ShowIcon(Sprite newIcon)
    {
        iconImage.sprite = newIcon;
        iconImage.enabled = true;
        isVisible = true;
    }

    public void HideIcon()
    {
        iconImage.enabled = false;
        isVisible = false;
    }
}
