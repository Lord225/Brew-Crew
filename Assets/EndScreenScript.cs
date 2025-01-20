using TMPro;
using Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI points;
    public CanvasGroup group;

    public CanvasGroup group2;
    public void ShowEndScreen(int points)
    {
        group.alpha = 1;
        group2.alpha = 0;
        
        this.points.text = "" + points * 100;
    }

    public void LoadMainMenu() {
        if (group.alpha == 0) return;

        SceneManager.LoadScene("Main Menu");
    }

    public void LoadMainMenuForced() {
        if (group.alpha == 1) return;

        SceneManager.LoadScene("Main Menu");
    }


    void Start() {
        group.alpha = 0;
        group2.alpha = 1;
    }
}
