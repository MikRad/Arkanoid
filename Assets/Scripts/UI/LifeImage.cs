using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LifeImage : MonoBehaviour
{
    private const int Available = 0;
    private const int NotAvailable = 1;
    
    [FormerlySerializedAs("image")] [SerializeField] private Image _image;
    [FormerlySerializedAs("sprites")] [SerializeField] private Sprite[] _sprites;
    
    public void SetAvailable(bool isAvailable)
    {
        _image.sprite = isAvailable ? _sprites[Available] : _sprites[NotAvailable];
    }

    public float GetWidth()
    {
        return _image.sprite.rect.width;
    }
}
