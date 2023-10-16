using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [Header("UI")]

    [SerializeField] private LifesView _lifesView;
    [SerializeField] private PauseView _pauseViewPrefab;
    [SerializeField] private GameOverView _gameOverViewPrefab;
    [SerializeField] private Transform _canvasTransform;
    [SerializeField] private Text _scoreText;
    
    private PauseView _pauseView;
    private GameOverView _gameOverView;

    [Header("Base settings")]

    [SerializeField] private LevelController _levelController;

    [Range(1, 5)]
    [SerializeField] private int _maxLifes = 3;
    [SerializeField] private bool _isAutoPlay;
    
    public static bool IsAutoPlay { get; private set; }
    public static bool IsPaused { get; private set; }

    private void OnEnable()
    {
        AddHandlers();
    }

    private void OnDisable()
    {
        RemoveHandlers();
    }
    
    private void Start()
    {
        IsAutoPlay = _isAutoPlay;
        IsPaused = false;

        GameStats stats = GameStats.Current;
        stats.StartCheck(_maxLifes);
        
        _lifesView.Init(stats.LifesLeft, stats.MaxLifes);
        
        UpdateScoreText();
    }
    
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // cheat for testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            AddLife();
        }
    }
    
    private void AddHandlers()
    {
        PauseView.OnClosed += HandlePauseViewClosed;
        GameOverView.OnClosed += HandleGameOverViewClosed;
        PickUpScore.OnPickUpScoreCollected += HandlePickUpScoreCollected;
        PickUpLifeUp.OnPickUpLifeUpCollected += HandlePickUpLifeUpCollected;
        _levelController.OnScoreEarned += ChangeScore;
        _levelController.OnLifeLost += HandleLifeLost;
        _levelController.OnLevelCompleted += HandleLevelCompleted;
    }

    private void RemoveHandlers()
    {
        PauseView.OnClosed -= HandlePauseViewClosed;
        GameOverView.OnClosed -= HandleGameOverViewClosed;
        PickUpScore.OnPickUpScoreCollected -= HandlePickUpScoreCollected;
        PickUpLifeUp.OnPickUpLifeUpCollected -= HandlePickUpLifeUpCollected;
        _levelController.OnScoreEarned -= ChangeScore;
        _levelController.OnLifeLost -= HandleLifeLost;
        _levelController.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void HandleLevelCompleted()
    {
        if (IsNextLevelAvailable())
        {
            MoveToNextLevel();
        }
        else
        {
            ExitToMainMenu();
        }
    }
    
    private void HandleLifeLost()
    {
        RemoveLife();
        
        if (GameStats.Current.HasAvailableLifes())
        {
            _levelController.SpawnBall();
        }
        else
        {
            // Game Over
            IsPaused = true;
            _gameOverView = Instantiate(_gameOverViewPrefab, _canvasTransform);
            _gameOverView.SetScore(GameStats.Current.TotalScore);
        }
    }
    
    private void HandlePauseViewClosed()
    {
        TogglePause();
    }

    private void HandleGameOverViewClosed()
    {
        Destroy(_gameOverView.gameObject);

        ExitToMainMenu();
    }

    private void HandlePickUpScoreCollected(PickUpScore pScore)
    {
        ChangeScore(pScore.Score);
    }

    private void HandlePickUpLifeUpCollected(PickUpLifeUp pLifeUp)
    {
        AddLife();
    }
    
    private void TogglePause()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;
            _pauseView = Instantiate(_pauseViewPrefab, _canvasTransform);
        }
        else
        {
            Time.timeScale = 1f;
            Destroy(_pauseView.gameObject);
        }
    }

    private void UpdateScoreText()
    {
        _scoreText.text = $"Score: {GameStats.Current.TotalScore}";
    }
    
    private void AddLife()
    { 
        GameStats stats = GameStats.Current;
        
        stats.AddLife();
        _lifesView.UpdateState(stats.LifesLeft);
    }
    
    private void RemoveLife()
    {
        GameStats stats = GameStats.Current;
        
        stats.RemoveLife();
        _lifesView.UpdateState(stats.LifesLeft);
    }
    
    private bool IsNextLevelAvailable()
    {
        return SceneManager.sceneCountInBuildSettings > SceneManager.GetActiveScene().buildIndex + 1;
    }
    
    private void MoveToNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    private void ExitToMainMenu()
    {
        GameStats.Current.Reset();
        
        SceneManager.LoadScene(0);
    }
    
    private void ChangeScore(int scoreAmount)
    {
        GameStats.Current.ChangeScore(scoreAmount);
        
        UpdateScoreText();
    }
}
