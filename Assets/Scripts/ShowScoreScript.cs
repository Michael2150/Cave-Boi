using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowScoreScript : MonoBehaviour
{
    private void Update()
    {
        GameSession sesh = FindObjectOfType<GameSession>();
        sesh.setUivisible(false);
        if (sesh.getPlayerLost())
        {
            sesh.getPlayer().playerDie();
        }
        else
        {
            sesh.getPlayer().playerWin();
        }
        Text t = GetComponent<Text>();
        int coinScore = Mathf.RoundToInt(sesh.getCoinScore());
        int lives = Mathf.RoundToInt(sesh.getLives());
        int livesScore = Mathf.RoundToInt(sesh.getScorePerLive());
        t.text = $"Coins - {coinScore} \nLives - {lives} x {livesScore} \n\nTotal - {coinScore + (lives * livesScore)}";
    }
}
