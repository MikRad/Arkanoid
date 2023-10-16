using System;
using UnityEngine;

public class PickUpBallSize : BasePickUp
{
    [SerializeField] private float _sizeFactor = 1.25f;

    public static event Action<PickUpBallSize> OnPickUpBallSizeCollected;

    public float SizeFactor => _sizeFactor;

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpBallSizeFX();

        OnPickUpBallSizeCollected?.Invoke(this);
    }
}
