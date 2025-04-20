using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public void Level1()
    {
        SceneManager.LoadSceneAsync("Level1");
    }
    public void Level2()
    {
        SceneManager.LoadSceneAsync("Level2");
    }
    public void Level3()
    {
        SceneManager.LoadSceneAsync("Level3");
    }
}
