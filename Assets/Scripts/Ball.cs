using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(TrailRenderer)), RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private Pad _pad;

    [Header("Launch settings")]
    [SerializeField] private Vector2 _minLaunchRandomDirection = new Vector2(-1f, 1f);
    [SerializeField] private Vector2 _maxLaunchRandomDirection = new Vector2(1f, 1f);
    [SerializeField] private float _defaultYOffsetFromPad = 0.7f;
    [SerializeField] private float _autoPlayLaunchDelay = 1f;

    [Header("Speed settings")]
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _minSpeed = 6f;
    [SerializeField] private float _maxSpeed = 12f;

    [Header("Size settings")]
    [SerializeField] private float _minSizeFactor = 0.5f;
    [SerializeField] private float _maxSizeFactor = 1.5f;

    private TrailRenderer _trailRenderer;
    private Rigidbody2D _rigidBody;
    private Transform _cachedTransform;
    private Transform _padTransform;

    private Coroutine _autoPlayLaunchRoutine;
    
    private Vector2 _launchOffsetFromPad;
    private bool _isLaunched;

    public static event Action<Ball> OnCreated;
    public static event Action<Ball> OnFailed;

    public float Speed => _speed;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
        
        _cachedTransform = transform;
    }

    private void OnEnable()
    {
        PickUpSpeed.OnPickUpSpeedCollected += HandlePickUpSpeed;
        PickUpBallSize.OnPickUpBallSizeCollected += HandlePickUpSize;
    }

    private void OnDisable()
    {
        PickUpSpeed.OnPickUpSpeedCollected -= HandlePickUpSpeed;
        PickUpBallSize.OnPickUpBallSizeCollected -= HandlePickUpSize;
    }

    private void Start()
    {
        _padTransform = _pad.transform;
        
        SetDefaultOffsetFromPad();
        
        OnCreated?.Invoke(this);
    }

    private void Update()
    {
        if (Game.IsPaused)
            return;

        if (!_isLaunched)
        {
            MoveWithPad();
            
            if(Game.IsAutoPlay && _autoPlayLaunchRoutine == null)
            {
                _autoPlayLaunchRoutine = StartCoroutine(AutoPlayLaunch());
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                Launch();
            }
        }
    }

    public void Launch()
    {
        _isLaunched = true;
        _trailRenderer.enabled = true;

        float dirX = Random.Range(_minLaunchRandomDirection.x, _maxLaunchRandomDirection.x);
        float dirY = Random.Range(_minLaunchRandomDirection.y, _maxLaunchRandomDirection.y);

        _rigidBody.velocity = (new Vector2(dirX, dirY).normalized) * _speed;
    }

    private IEnumerator AutoPlayLaunch()
    {
        yield return new WaitForSeconds(_autoPlayLaunchDelay);
        
        Launch();
        _autoPlayLaunchRoutine = null;
    }
    
    private void MoveWithPad()
    {
        Vector3 pos = _padTransform.position + (Vector3)_launchOffsetFromPad;
        
        _cachedTransform.position = pos;
    }
    
    private void Stop()
    {
        _isLaunched = false;
        _trailRenderer.enabled = false;

        _rigidBody.velocity = Vector2.zero;
        _rigidBody.angularVelocity = 0f;
        
        CalculateOffsetFromPad();
    }

    private void SetDefaultOffsetFromPad()
    {
        _launchOffsetFromPad = new Vector2(0f, _defaultYOffsetFromPad);
    }
    
    private void CalculateOffsetFromPad()
    {
        _launchOffsetFromPad = new Vector2(_cachedTransform.position.x - _padTransform.position.x, _defaultYOffsetFromPad);
    }
    
    private void ChangeSpeed(float speedFactor)
    {
        _speed *= speedFactor;
        _speed = Mathf.Clamp(_speed, _minSpeed, _maxSpeed);

        UpdateSpeed();
    }
    
    private void ChangeSize(float sizeFactor)
    {
        Vector3 scaleValue = _cachedTransform.localScale;
        scaleValue.x *= sizeFactor;
        scaleValue.x = Mathf.Clamp(scaleValue.x, _minSizeFactor, _maxSizeFactor);
        scaleValue.y = scaleValue.x;
        _cachedTransform.localScale = scaleValue;
    }

    private void HandlePickUpSpeed(PickUpSpeed ps)
    {
        ChangeSpeed(ps.SpeedFactor);
    }

    private void HandlePickUpSize(PickUpBallSize pbs)
    {
        ChangeSize(pbs.SizeFactor);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_pad.IsSticky && collision.gameObject.CompareTag(Tags.Pad))
        {
            Stop();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_isLaunched)
        {
            UpdateSpeed();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(Tags.Floor))
        {
            OnFailed?.Invoke(this);
            Destroy(gameObject);
        }
    }
    
    private void UpdateSpeed()
    {
        _rigidBody.velocity = _speed * (_rigidBody.velocity.normalized);
    }
}
