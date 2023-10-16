using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Pad : MonoBehaviour
{
    [Header("Base settings")]
    [SerializeField] private float _minWidthFactor = 0.5f;
    [SerializeField] private float _maxWidthFactor = 1.5f;
    [SerializeField] private float _autoPlayBallFollowSpeedCoeff = 1.5f;

    private SpriteRenderer _spriteRenderer;
    private Transform _cachedTransform;
    private Camera _mainCamera;
    
    // for autoplay mode
    private Transform _ballTransform;
    private float _autoPlayMoveSpeed;

    private float _horizontalMoveLimit;
    private float _stickyEffectDuration;
    
    public bool IsSticky { get; private set; }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        _cachedTransform = transform;
    }

    private void OnEnable()
    {
        Ball.OnCreated += HandleBallCreated;
        PickUpSticky.OnPickUpStickyCollected += HandlePickUpStickyCollected;
        PickUpPadWidth.OnPickUpPadWidthCollected += HandlePickUpWidthCollected;
    }

    private void OnDisable()
    {
        Ball.OnCreated -= HandleBallCreated;
        PickUpSticky.OnPickUpStickyCollected -= HandlePickUpStickyCollected;
        PickUpPadWidth.OnPickUpPadWidthCollected -= HandlePickUpWidthCollected;
    }
    
    private void Start()
    {
        _mainCamera = Camera.main;
        
        CalculateHorizontalMoveLimit();
    }

    private void Update()
    {
        if (Game.IsPaused)
            return;
        
        UpdatePosition();
        
        if (IsSticky)
        {
            UpdateStickyEffect();
        }
    }
    
    private void CalculateHorizontalMoveLimit()
    {
        Vector2 screenSizeWorld = _mainCamera.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
        float padWidthWorld = _spriteRenderer.bounds.size.x;

        _horizontalMoveLimit = screenSizeWorld.x - padWidthWorld / 2;
    }

    private void HandleBallCreated(Ball ball)
    {
        _ballTransform = ball.transform;

        _autoPlayMoveSpeed = ball.Speed * _autoPlayBallFollowSpeedCoeff;
    }

    private void HandlePickUpStickyCollected(PickUpSticky ps)
    {
        IsSticky = true;
        _stickyEffectDuration = ps.EffectDuration;
    }

    private void HandlePickUpWidthCollected(PickUpPadWidth pw)
    {
        ChangeWidth(pw.WidthFactor);
    }

    private void ChangeWidth(float widthFactor)
    {
        Vector3 size = _cachedTransform.localScale;
        size.x *= widthFactor;
        size.x = Mathf.Clamp(size.x, _minWidthFactor, _maxWidthFactor);
        _cachedTransform.localScale = size;

        CalculateHorizontalMoveLimit();
    }

    private void UpdatePosition()
    {
        if(Game.IsAutoPlay)
        {
            AutoPlayFollowBall();
        }
        else
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector3 padPos = new Vector3(mouseWorldPos.x, _cachedTransform.position.y, 0);
        padPos.x = Mathf.Clamp(padPos.x, -_horizontalMoveLimit, _horizontalMoveLimit);

        _cachedTransform.position = padPos;
    }

    private void AutoPlayFollowBall()
    {
        Vector3 currentPos = _cachedTransform.position;
        Vector3 targetPos = new Vector3(_ballTransform.position.x, currentPos.y, 0);
        targetPos.x = Mathf.Clamp(targetPos.x, -_horizontalMoveLimit, _horizontalMoveLimit);

        currentPos =
            Vector2.MoveTowards(currentPos, targetPos, Time.deltaTime * _autoPlayMoveSpeed);
        _cachedTransform.position = currentPos;
    }

    private void UpdateStickyEffect()
    {
        _stickyEffectDuration -= Time.deltaTime;

        if(_stickyEffectDuration <= 0)
        {
            IsSticky = false;
        }
    }
}
