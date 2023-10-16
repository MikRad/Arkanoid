using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Views")]
    [SerializeField] private GameObject _startView;
    [SerializeField] private GameObject _levelsView;
    
    [Header("Buttons")]
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _backButton;

    [SerializeField] private Button[] _levelButtons;

    private void Start()
    {
        _levelsView.SetActive(false);

        _playButton.onClick.AddListener(HandlePlayClick);
        _exitButton.onClick.AddListener(HandleExitClick);
        _backButton.onClick.AddListener(HandleBackClick);

        for (int i = 0; i < _levelButtons.Length; i++)
        {
            int levelBtnIdx = i;
            _levelButtons[levelBtnIdx].onClick.AddListener(() => { HandleLevelButtonClick(levelBtnIdx + 1); });
        }
    }

    private void HandlePlayClick()
    {
        _startView.SetActive(false);
        _levelsView.SetActive(true);
    }

    private void HandleExitClick()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void HandleBackClick()
    {
        _levelsView.SetActive(false);
        _startView.SetActive(true);
    }

    private void HandleLevelButtonClick(int levelNumber)
    {
        if (SceneManager.sceneCountInBuildSettings > levelNumber)
        {
            SceneManager.LoadScene(levelNumber);
        }
    }
}
