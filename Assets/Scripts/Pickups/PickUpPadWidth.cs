using System;
using UnityEngine;

public class PickUpPadWidth : BasePickUp
{
    [SerializeField] private float _widthFactor = 1.25f;

    public static event Action<PickUpPadWidth> OnPickUpPadWidthCollected;

    public float WidthFactor => _widthFactor;

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpPadWidthFX(this);

        OnPickUpPadWidthCollected?.Invoke(this);
    }
}
