using System;

public class PickUpLifeUp : BasePickUp
{
    public static event Action<PickUpLifeUp> OnPickUpLifeUpCollected;

    protected override void ApplyEffect()
    {
        FxController.Instance.PlayPickUpLifeFX(this);

        OnPickUpLifeUpCollected?.Invoke(this);
    }
}
