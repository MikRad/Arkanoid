using System;
using UnityEngine;

public class PickUpSticky : BasePickUp
{
    [SerializeField] private float _effectDuration = 30f;

    public float EffectDuration => _effectDuration;

    public static event Action<PickUpSticky> OnPickUpStickyCollected;

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpStickyFX();

        OnPickUpStickyCollected?.Invoke(this);
    }
}
