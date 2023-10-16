using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private readonly List<Ball> _activeBalls = new List<Ball>();
    private Ball _lastActiveBall;
    
    private int _blocksNumberToFinishLevel;
    
    public event Action<int> OnScoreEarned; 
    public event Action OnLifeLost; 
    public event Action OnLevelCompleted; 

    private void OnEnable()
    {
        AddHandlers();
    }

    private void OnDisable()
    {
        RemoveHandlers();
    }

    public void SpawnBall()
    {
        SpawnBall(_lastActiveBall);
    }
    
    private void AddHandlers()
    {
        Block.OnCreated += HandleBlockCreate;
        Block.OnDestroyed += HandleBlockDestroy;
        Ball.OnFailed += HandleBallLoss;
        Ball.OnCreated += HandleBallCreate;
        PickUpAddBalls.OnPickUpAddBallsCollected += HandlePickUpAddBallsCollected;
    }

    private void RemoveHandlers()
    {
        Block.OnCreated -= HandleBlockCreate;
        Block.OnDestroyed -= HandleBlockDestroy;
        Ball.OnFailed -= HandleBallLoss;
        Ball.OnCreated -= HandleBallCreate;
        PickUpAddBalls.OnPickUpAddBallsCollected -= HandlePickUpAddBallsCollected;
    }
    
    private void HandleBallLoss(Ball ball)
    {
        bool isAllBallsLost = RemoveActiveBall(ball);

        if(isAllBallsLost)
        {
            _lastActiveBall = ball;
            OnLifeLost?.Invoke();
        }
    }
    
    private Ball SpawnBall(Ball ball)
    {
        return Instantiate(ball);
    }

    private void HandleBallCreate(Ball ball)
    {
        _activeBalls.Add(ball);
    }

    private void HandleBlockCreate(Block block)
    {
        if (!block.IsUndestroyable)
        {
            _blocksNumberToFinishLevel++;
        }
    }

    private void HandleBlockDestroy(Block block)
    {
        if(!block.IsUndestroyable)
        {
            _blocksNumberToFinishLevel--;
            OnScoreEarned?.Invoke(block.ScoreForDestroy);
        }

        if (_blocksNumberToFinishLevel == 0)
        {
            OnLevelCompleted?.Invoke();
        }
    }
    
    private void HandlePickUpAddBallsCollected(PickUpAddBalls pab)
    {
        foreach (Ball ball in _activeBalls)
        {
            for (int i = 0; i < pab.BallsNumberToAdd; i++)
            {
                SpawnBall(ball).Launch();
            }
        }    
    }
    
    private bool RemoveActiveBall(Ball ball)
    {
        _activeBalls.Remove(ball);

        return _activeBalls.Count == 0;
    }
}