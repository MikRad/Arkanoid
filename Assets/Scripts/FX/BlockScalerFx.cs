using Lean.Pool;
using UnityEngine;

public class BlockScalerFx : MonoBehaviour, IPoolable
{
    [Header("Base Settings")]
    [SerializeField] private float _lifeTime = 0.2f;

    [Range(0f, 1f)]
    [SerializeField] private float _additionalScaleFactor = 0.1f;

    private readonly Vector3 _defaultScale = Vector3.one;

    private float _timeToFinish;
    private Transform _targetTransform;

    private void Update()
    {
        if (_targetTransform == null)
        {
            FinishEffect();
            return;
        }
        
        UpdateScale();
        UpdateLifeTime();
    }

    public void OnSpawn()
    {
        _timeToFinish = _lifeTime;
    }

    public void OnDespawn()
    {
    }
    
    public void SetTarget(Transform target)
    {
        _targetTransform = target;
    }
    
    private void UpdateLifeTime()
    {
        _timeToFinish -= Time.deltaTime;
        
        if (_timeToFinish <= 0)
        {
            FinishEffect();
        }
    }

    private void FinishEffect()
    {
        if (_targetTransform != null)
        {
            _targetTransform.localScale = _defaultScale;
            _targetTransform = null;
        }

        LeanPool.Despawn(gameObject);
    }

    private void UpdateScale()
    {
        float scaleDelta = _additionalScaleFactor * (Mathf.Sin(2 * Mathf.PI * _timeToFinish / _lifeTime));
        _targetTransform.localScale = new Vector3(1f + scaleDelta, 1f + scaleDelta, 0);
    }
}
