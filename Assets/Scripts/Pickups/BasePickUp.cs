using UnityEngine;

public abstract class BasePickUp : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Pad))
        {
            ApplyEffect();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag(Tags.Floor))
        {
            Destroy(gameObject);
        }
    }

    protected abstract void ApplyEffect();
}
