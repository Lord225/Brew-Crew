using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public void loadTuotorial()
    {
        SceneManager.LoadScene("Tutorial");;
    }

    public void loadLevel1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void loadLevel2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void loadLevel3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void loadLevel4()
    {
        SceneManager.LoadScene("Level4");
    }
    
}
