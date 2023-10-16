using Lean.Pool;
using TMPro;
using UnityEngine;

public class FlyingMessage : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private TextMeshPro _messageText;
    [SerializeField] private Color _positiveColor = new Color(0.0f, 1.0f, 0.0f);
    [SerializeField] private Color _negativeColor = new Color(1.0f, 0.0f, 0.0f);

    [Header("Flying Settings")]
    [SerializeField] private float _flyingSpeed = 2f;
    [SerializeField] private float _lifeTime = 1.5f;

    [Header("Scale Settings")]
    [SerializeField] private float _scaleTime = 0.5f;
    [SerializeField] private float _scaleMaxPercent = 20f;

    private float _timeToDisappear;
    private float _scaleFactor;
    private bool _isScaleCompleted;

    private Transform _cachedTransform;

    private void Awake()
    {
        _cachedTransform = transform;
    }

    private void Update()
    {
        UpdateLifeTime();

        UpdatePosition();
        UpdateColor();
        UpdateScale();
    }
    
    public void Init(Vector2 position, string text, bool isPositiveSense)
    {
        _messageText.text = text;
        _timeToDisappear = _lifeTime;

        _messageText.color = isPositiveSense ? _positiveColor : _negativeColor;
        
        _cachedTransform.position = position;
        _scaleFactor = _scaleMaxPercent / 100;
    }
    
    private void UpdatePosition()
    {
        _cachedTransform.Translate(0, _flyingSpeed * Time.deltaTime, 0);
    }

    private void UpdateColor()
    {
        Color color = _messageText.color;
        float alpha = (1f + _timeToDisappear / _lifeTime) / 2;
        color.a = alpha;
        _messageText.color = color;
    }
    
    private void UpdateScale()
    {
        if(!_isScaleCompleted)
        {
            float scaleDelta = _scaleFactor * Mathf.Sin(Mathf.PI * ((_lifeTime - _timeToDisappear) / _scaleTime));
            _cachedTransform.localScale = new Vector3(1f + scaleDelta, 1f + scaleDelta, 0);

            if (_lifeTime - _timeToDisappear >= _scaleTime)
            {
                _isScaleCompleted = true;
            }
        }
    }
    
    private void UpdateLifeTime()
    {
        _timeToDisappear -= Time.deltaTime;
        
        if (_timeToDisappear <= 0)
        {
            LeanPool.Despawn(gameObject);
        }
    }
}
