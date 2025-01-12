using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class IconScript : MonoBehaviour
{
    private Camera mainCamera;
    private GameObject icon;
    public Vector3 offset = new Vector3(0, 0.35f, 0);
    public Color color = Color.white;
    public Sprite sprite;
    public float scale = 1.0f;
    public float rotation =  0.0f;
    public bool showIcon = true;
    public string description = "";

    void Start() 
    {
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();

        // get canvas and add prefab named Assets/UI/icon , link them together
        GameObject canvas = GameObject.Find("Canvas");
        GameObject icon = Instantiate(Resources.Load("icon", typeof(GameObject))) as GameObject;
        this.icon = icon;
        this.icon.SetActive(false);
        this.icon.GetComponent<Image>().sprite = sprite;

        // set parent of icon to canvas
        icon.transform.SetParent(canvas.transform, false);
    }

    // Update is called once per frame
    void Update()
    {
        if (icon != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position + offset);
            icon.transform.position = screenPos;

            icon.transform.localScale = new Vector3(scale, scale, scale);

            icon.GetComponent<Image>().color = color;
            icon.GetComponent<Image>().sprite = sprite;
            icon.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, rotation);

            if (showIcon)
                icon.SetActive(true);
            else
                icon.SetActive(false);

            var text = icon.GetComponentInChildren<TextMeshProUGUI>();
            if(text != null) {
                if (description != "")
                    text.enabled = true;
                else
                    text.enabled = false;
        
                if (text.text != description)
                    text.text = description;
            }
        }
    }

    void OnDestroy()
    {
        if (icon != null)
        {
            Destroy(icon);
        }
    }
}