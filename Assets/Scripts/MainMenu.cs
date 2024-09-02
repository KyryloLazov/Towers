using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{

    public string levelToLoad = "LevelSelect";
    public GameObject Menu;
    public GameObject Difficulty;

    public void Play()
    {
        if(PlayerPrefs.GetInt("isChosen") == 1)
        {
            SceneManager.LoadScene(levelToLoad);
        }
        else
        {
            ShowDifficulty();
        }
    }

    public void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    private void ShowDifficulty()
    {
        Menu.SetActive(false);
        Difficulty.SetActive(true);
    }

    public void DefaultDifficulty()
    {
        PlayerPrefs.SetInt("isHard", 0);
        PlayerPrefs.SetInt("isChosen", 1);
        SceneManager.LoadScene(levelToLoad);
    }

    public void HardDifficulty()
    {
        PlayerPrefs.SetInt("isHard", 1);
        PlayerPrefs.SetInt("isChosen", 1);
        SceneManager.LoadScene(levelToLoad);
    }


}
