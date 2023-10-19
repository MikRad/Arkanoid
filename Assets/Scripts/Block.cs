using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Block : MonoBehaviour
{
    [Header("Block View")]
    [SerializeField] private Sprite[] _stateSprites;

    [Header("Base Settings")]
    [SerializeField] private int _scoreForDestroy = 5;
    [SerializeField] private int _hitsNumberToDestroy = 1;

    [Header("Block Type")]
    [SerializeField] private BlockType _type;
    [SerializeField] private bool _isUnderstroyable;
    [SerializeField] private bool _isInvisible;

    [Header("Explosion")]
    [SerializeField] private float _explosionRadius;

    [Header("Bonus Generation")]
    [SerializeField] private GameObject[] _pickUpPrefabs;

    [Range (0, 100)]
    [SerializeField] private int _pickupGenerationProbability = 30;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;
    private Transform _cachedTransform;

    private int _currentHitsNumber;
    
    public static event Action<Block> OnDestroyed;
    public static event Action<Block> OnCreated;

    public bool IsUndestroyable => _isUnderstroyable;
    public int ScoreForDestroy => _scoreForDestroy;
    public BlockType Type => _type;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _cachedTransform = transform;
    }

    private void Start()
    {
        UpdateView();

        if(_isInvisible)
        {
            Hide();
        }

        OnCreated?.Invoke(this);
    }

    private void OnCollisionEnter2D()
    {
        if(_isInvisible)
        {
            FxController.Instance.PlayBlockHitFX(this);
            Show();
            return;
        }

        if(!_isUnderstroyable)
        {
            FxController.Instance.PlayBlockHitFX(this);
            HandleHit();
        }
    }

    private void OnDrawGizmos()
    {
        if (_explosionRadius > 0)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }

    private void HandleHit()
    {
        _currentHitsNumber++;
        if(_currentHitsNumber == _hitsNumberToDestroy)
        {
            Destroy();
        }
        else
        {
            UpdateView();
        }
    }

    private void Show()
    {
        Color color = _spriteRenderer.color;
        color.a = 1.0f;
        _spriteRenderer.color = color;
        
        _isInvisible = false;
    }
    
    private void Hide()
    {
        Color color = _spriteRenderer.color;
        color.a = 0.0f;
        _spriteRenderer.color = color;
        
        _isInvisible = true;
    }
    
    private void Destroy()
    {
        Destroy(gameObject);

        if (_type == BlockType.Explosive)
        {
            LaunchExplosion();
        }

        FxController.Instance.PlayBlockDestroyFX(this);
        
        TryGeneratePickUp();

        OnDestroyed?.Invoke(this);
    }

    private void HandleDelayedDestroy(float delay)
    {
        StartCoroutine(UpdateDelayedDestroy(delay));
    }

    private IEnumerator UpdateDelayedDestroy(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Destroy();
    }
    
    private void LaunchExplosion()
    {
        _collider2D.enabled = false;
        
        LayerMask lMask = LayerMask.GetMask(LayerNames.Block);
        Collider2D[] objectsInRadius = Physics2D.OverlapCircleAll(_cachedTransform.position, _explosionRadius, lMask);

        for (int i = 0; i < objectsInRadius.Length; i++)
        {
            Collider2D obj = objectsInRadius[i];
            if (obj.gameObject.TryGetComponent(out Block block))
            {
                block.HandleDelayedDestroy(i * 0.05f);
            }
        }
    }
    
    private void UpdateView()
    {
        if(_currentHitsNumber < _stateSprites.Length)
        {
            _spriteRenderer.sprite = _stateSprites[_currentHitsNumber];
        }
    }

    private void TryGeneratePickUp()
    {
        if (IsNeedToCreatePickUp())
        {
            int randIndex = Random.Range(0, _pickUpPrefabs.Length);
            Instantiate(_pickUpPrefabs[randIndex], _cachedTransform.position, Quaternion.identity);
        }
    }
    
    private bool IsNeedToCreatePickUp()
    {
        if ((_pickUpPrefabs == null) || (_pickUpPrefabs.Length == 0))
            return false;

        int randNum = Random.Range(1, 101);

        return randNum <= _pickupGenerationProbability; 
    }
}
