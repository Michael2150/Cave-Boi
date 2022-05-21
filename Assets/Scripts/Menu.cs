using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void Start()
    {
        FindObjectOfType<GameSession>().setUivisible(false);
    }
    public void loadFirstLevel()
    {
        FindObjectOfType<GameSession>().startNewGame();
        FindObjectOfType<GameSession>().setUivisible(true);
        SceneManager.LoadScene(1);
    }
    public void loadMenuLevel()
    {
        FindObjectOfType<GameSession>().setUivisible(false);
        SceneManager.LoadScene(0);
    }
    public void quit()
    {
        Application.Quit(0);
    }
    public void loadSecondLevel()
    {
        loadLevel(true, 2);
    }
    public void loadLevel(bool uiVisible ,int levelIndex)
    {
        FindObjectOfType<GameSession>().setUivisible(uiVisible);
        SceneManager.LoadScene(levelIndex);
    }
}
