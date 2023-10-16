using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PickUpScore : BasePickUp
{
    [SerializeField] private int[] _possibleScores = {5, 10, 15, 20, 25};

    public static event Action<PickUpScore> OnPickUpScoreCollected;

    public int Score { get; private set; }

    private void Start()
    {
        int randIndex = Random.Range(0, _possibleScores.Length);
        Score = _possibleScores[randIndex];
    }

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpScoreFX(this);

        OnPickUpScoreCollected?.Invoke(this);
    }
}
