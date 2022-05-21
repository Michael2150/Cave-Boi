using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{
    //UI overlay
    [SerializeField] Text scoreText;
    [SerializeField] float scorePerCoin = 100f;
    [SerializeField] float scorePerLive = 250f;
    [SerializeField] GameObject UIHud;
    [SerializeField] GameObject healthBar;
    [SerializeField] GameObject oxygenBar;
    private bool oxygenBarVisible;

    //Player stats
    [SerializeField] float oxygenLevel = 10f;
    [SerializeField] float startingPlayerLives = 5f;
    [SerializeField] float playerLives = 5f;
    [SerializeField] int coins = 0;
    [SerializeField] bool hasKey;
    [SerializeField] bool playerLost;

    private void Awake()
    {
        //Checks if there are multiple gamesession objects and destroys this if there.
        if (FindObjectsOfType<GameSession>().Length > 1) {
            Destroy(gameObject);
        } else {
            DontDestroyOnLoad(gameObject); 
        }
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void Update()
    {
        //Updates the ui based on the player stats
        oxygenBar.GetComponent<UIBarScript>().setVisible(oxygenBarVisible);
        oxygenBar.GetComponent<UIBarScript>().updateStat(oxygenLevel);
        healthBar.GetComponent<UIBarScript>().updateStat(playerLives);
        scoreText.text = "x " + (coins).ToString();
    }

    public void handlePlayerDeath()
    {
        playerLives--;
        StartCoroutine(respawnPlayer());
    }
    IEnumerator respawnPlayer()
    {
        yield return new WaitForSecondsRealtime(2f);

        if (playerLives <= 0)
        {
            //Show score screen
            if (!playerLost)
            {
                playerLost = true;
                SceneManager.LoadScene(SceneManager.sceneCountInBuildSettings - 1);
            }
        }
        else
        {
            ArrayList spawnpoints = new ArrayList(FindObjectsOfType<SpawnpointScript>());
            Vector2 playerRespawnPoint = Vector2.zero;
            foreach (SpawnpointScript sp in spawnpoints)
            {
                if (sp.isActive())
                {
                    playerRespawnPoint = sp.getPos();
                    break;
                }
                else if (sp.isMainSpawnpoint())
                {
                    playerRespawnPoint = sp.getPos();
                }
            }
            PlayerScript player = getPlayer();
            player.transform.position = playerRespawnPoint;
            player.playerReset();
        }
    }

    public void handlePlayerCollectCoin()
    {
        coins++;
    }

    //This is called by the player object to update the ui
    public void setPlayerOxygenLevel(float curOxygenLevel, float maxOxygenLevel)
    {
        oxygenBarVisible = (curOxygenLevel != maxOxygenLevel);
        oxygenLevel = curOxygenLevel;
    }

    public void handlePlayerCollectKey()
    {
        hasKey = true;
    }

    public bool handlePlayerTouchDoor()
    {
        if (hasKey)
        {
            //loads the next scene
            getPlayer().playerWin();
            hasKey = false;
            StartCoroutine(nextLevel());
            return true;
        }
        return false;
    }
    IEnumerator nextLevel()
    {
        yield return new WaitForSecondsRealtime(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void handlePlayerPickupExtraLife()
    {
        playerLives++;
    }

    public PlayerScript getPlayer()
    {
        return FindObjectOfType<PlayerScript>();
    }
    public float getCoinScore()
    {
        return coins * scorePerCoin;
    }
    public float getLives()
    {
        return playerLives;
    }
    public float getScorePerLive()
    {
        return scorePerLive;
    }
    public bool getPlayerLost()
    {
        return playerLost;
    }

    public void startNewGame()
    {
        oxygenBarVisible = false;
        coins = 0;
        playerLives = startingPlayerLives;
        hasKey = false;
        playerLost = false;
    }
    public void setUivisible(bool visible)
    {
        UIHud.SetActive(visible);
    }
}
