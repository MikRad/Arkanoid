using System;
using UnityEngine;

public class PickUpSpeed : BasePickUp
{
    [SerializeField] private float _speedFactor = 1.2f;

    public static event Action<PickUpSpeed> OnPickUpSpeedCollected;

    public float SpeedFactor => _speedFactor;

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpSpeedFX(this);

        AudioController.Instance.PlaySfx(_speedFactor < 1 ? SfxType.PickUpCollect : SfxType.PickUpNegativeCollect);

        OnPickUpSpeedCollected?.Invoke(this);
    }
}
