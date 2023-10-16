using System.Collections.Generic;
using UnityEngine;

public class LifesView : MonoBehaviour
{
    [SerializeField] private LifeImage _lifeImagePrefab;
    [SerializeField] private bool _isCentered = true;
    [SerializeField] private int _offsetXBetweenImages = 10;

    private readonly List<LifeImage> _lifeImages = new List<LifeImage>();

    public void Init(int lifesLeft, int maxLifes)
    {
        for (int i = 0; i < maxLifes; i++)
        {
            LifeImage image = Instantiate(_lifeImagePrefab, transform);
            image.SetAvailable(i < lifesLeft);
            _lifeImages.Add(image);
        }
        
        AlignImages();
    }

    public void UpdateState(int lifesLeft)
    {
        for (int i = 0; i < _lifeImages.Count; i++)
        {
            LifeImage image = _lifeImages[i];
            image.SetAvailable(i < lifesLeft);
        }
    }

    private void AlignImages()
    {
        float offsetX = CalculateOffsetXFromParent();

        for (int i = 0; i < _lifeImages.Count; i++)
        {
            LifeImage image = _lifeImages[i];
            Vector2 pos = new Vector2(offsetX + (image.GetWidth() + _offsetXBetweenImages) * i, 0);
            image.transform.localPosition = pos;
        }
    }
    
    private float CalculateOffsetXFromParent()
    {
        if(_isCentered)
        {
            float totalWidth = (_lifeImagePrefab.GetWidth() + _offsetXBetweenImages) * (_lifeImages.Count - 1);

            return -(totalWidth / 2);
        }

        return 0;
    }
}
