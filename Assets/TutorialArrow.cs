using UnityEngine;

public class TutorialArrow : MonoBehaviour
{
    void goTo(string name) {
        transform.localPosition = GameObject.Find(name).transform.localPosition;
    }

    public void Cup() {
        goTo("Tut-Cup");
    }

    public void Coffe(){
        goTo("Tut-Coffe");
    }

    public void Top(){
        goTo("Tut-Top");
    }

    public void CoffeMachine() {
        goTo("Tut-CoffeMachine");
    }

    public void Steamer(){
        goTo("Tut-Steamer");
    }

    public void Milk(){
        goTo("Tut-Milk");
    }

    public void Table(){
        goTo("Tut-Table");
    }

    public void hide(){
        transform.localPosition = new Vector3(0, 1000, 0);
    }


}
