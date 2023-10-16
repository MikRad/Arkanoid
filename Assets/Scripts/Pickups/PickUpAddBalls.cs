using System;
using UnityEngine;

public class PickUpAddBalls : BasePickUp
{
    [SerializeField] private int _ballsNumberToAdd = 2;

    public static event Action<PickUpAddBalls> OnPickUpAddBallsCollected;

    public float BallsNumberToAdd => _ballsNumberToAdd;

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpAddBallsFX();

        OnPickUpAddBallsCollected?.Invoke(this);
    }
}
