using TMPro;
using UnityEngine;
using static ClientScript;

public class OrderElementScript : MonoBehaviour
{
    public ClientScript client;
    private Transform  hourglassIcon;
    private Transform ok;

    void updateIcon(State state) {
        Debug.Log("Updating icon - " + state);
        if (state == State.WaitingForOrder) {
            hourglassIcon.gameObject.SetActive(true);
            ok.gameObject.SetActive(false);
        } else if (state == State.Eating) {
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
     
        client.OnStateChangeHandler += (State newState) =>
        {
            updateIcon(newState);
        };

        updateIcon(client.currentState);
    }

    void OnDestroy()
    {
        client.OnStateChangeHandler = null;
    }
}
