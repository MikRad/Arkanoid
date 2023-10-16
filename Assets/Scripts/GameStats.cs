using UnityEngine;

public class GameStats
{
    private static GameStats _instance;
    
    public int TotalScore { get; private set; }
    public int LifesLeft { get; private set; }
    public int MaxLifes { get; private set; }

    public static GameStats Current
    {
        get
        {
            if (_instance == null)
            { 
                _instance = new GameStats();
            }

            return _instance;
        }
    }
    
    private GameStats(){}
    
    public void StartCheck(int maxLifes)
    {
        MaxLifes = maxLifes;
        
        if (LifesLeft == 0)
        {
            LifesLeft = maxLifes;
            TotalScore = 0;
        }
    }

    public void Reset()
    {
        LifesLeft = 0;
        TotalScore = 0;
    }
    
    public void AddLife()
    {
        if (LifesLeft < MaxLifes)
        {
            LifesLeft++;
        }
    }
    
    public void RemoveLife()
    {
        if (LifesLeft > 0)
        {
            LifesLeft--;
        }
    }
    
    public void ChangeScore(int scoreAmount)
    {
        TotalScore += scoreAmount;
        
        TotalScore = Mathf.Max(TotalScore, 0);
    }

    public bool HasAvailableLifes()
    {
        return LifesLeft > 0;
    }
}