using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject GameOverUI;
    public GameObject CompleteLeveUI;
    public static bool isGameOver;
    public static float difficultyRate = 1;
    

    private void Start()
    {
        isGameOver = false;

        if (PlayerPrefs.GetInt("isHard") == 1)
        {
            difficultyRate = 1.5f;
        }
    }

    void Update()
    {
        if (isGameOver)
        {
            return;
        }

        if(PlayerStats.Lives <= 0)
        {
            EndGame();
        }
    }

    void EndGame()
    {
        isGameOver = true;
       
        GameOverUI.SetActive(true);
    }

    public void WinLevel()
    {
        isGameOver = true;

        CompleteLeveUI.SetActive(true);
    }
}
