using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class OrderElementScript : MonoBehaviour
{
    public ClientScript client;
    private Transform  hourglassIcon;
    private Transform ok;

    void updateIcon(ClientScript.State state) {
        if(client.IsDestroyed() || this.IsDestroyed()) {
            return;
        }

        if (state == ClientScript.State.WaitingForOrder) {
            hourglassIcon.gameObject.SetActive(true);
            ok.gameObject.SetActive(false);
        } else if (state == ClientScript.State.Eating) {
            hourglassIcon.gameObject.SetActive(false);
            ok.gameObject.SetActive(true);
        } else {
            hourglassIcon.gameObject.SetActive(false);
            ok.gameObject.SetActive(false);
        }   
    }

    public void SetClient(ClientScript client)
    {
        this.client = client;
        
        hourglassIcon = transform.Find("hourglass");
        ok = transform.Find("Ok");

        if(TryGetComponent(out TextMeshProUGUI text)) {
            text.text = MugState.StateToString(client.order.requestedMug);
        } else {
            Debug.LogError("No text component found");
        }
     
        client.OnStateChangeHandler += updateIcon;

        updateIcon(client.currentState);
    }

    void OnDestroy()
    {
        if(client != null) {
            client.OnStateChangeHandler -= updateIcon;
        }
    }
}
