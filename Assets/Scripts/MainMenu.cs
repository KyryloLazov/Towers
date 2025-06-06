using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private string _levelToLoad = "LevelSelect";
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private GameObject _difficulty;
    
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _defaultDifficultyButton;
    [SerializeField] private Button _hardDifficultyButton;

    private void OnEnable()
    {
        _playButton.onClick.AddListener(Play);
        _exitButton.onClick.AddListener(Exit);
        _defaultDifficultyButton.onClick.AddListener(SetDefaultDifficulty);
        _hardDifficultyButton.onClick.AddListener(SetHardDifficulty);
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveAllListeners();
        _exitButton.onClick.RemoveAllListeners();
        _defaultDifficultyButton.onClick.RemoveAllListeners();
        _hardDifficultyButton.onClick.RemoveAllListeners();
    }

    private void Play()
    {
        ShowDifficulty();
    }

    private void Exit()
    {
        Debug.Log("Exit");
        Application.Quit();
    }

    private void ShowDifficulty()
    {
        _menuPanel.SetActive(false);
        _difficulty.SetActive(true);
    }

    private void SetDefaultDifficulty()
    {
        PlayerPrefs.SetInt("isHard", 0);
        SceneManager.LoadScene(_levelToLoad);
    }

    private void SetHardDifficulty()
    {
        PlayerPrefs.SetInt("isHard", 1);
        SceneManager.LoadScene(_levelToLoad);
    }
}
