using System;
using UnityEngine;
using UnityEngine.UI;

public class GameOverView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private Text _scoreText;

    public static event Action OnClosed;

    private void Start()
    {
        _playAgainButton.onClick.AddListener(PlayAgainClickHandler);
        _exitButton.onClick.AddListener(ExitClickHandler);
    }

    public void SetScore(int score)
    {
        _scoreText.text = $"Total score: {score}";
    }
    
    private void PlayAgainClickHandler()
    {
        OnClosed?.Invoke();
    }

    private void ExitClickHandler()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif    
    }
}
