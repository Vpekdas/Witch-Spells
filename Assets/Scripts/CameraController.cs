using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController Player;
    private readonly float _smoothSpeed = 0.125f;
    private readonly float _offsetSmoothSpeed = 0.2f;
    private readonly float _horizontalOffsetAmount = 0.5f;
    private readonly float _maxFallYOffset = -5.0f;
    private float _playerVelocityY;
    private Vector3 _velocity;
    private Vector3 _currentHorizontalOffset;
    private Vector3 _targetHorizontalOffset;
    private Vector3 _currentVerticalOffset;
    private Vector3 _targetVerticalOffset;

    private void Awake()
    {
        _velocity = Vector3.zero;
        _currentHorizontalOffset = Vector3.zero;
        _targetHorizontalOffset = Vector3.zero;
        _currentVerticalOffset = Vector3.zero;
        _targetVerticalOffset = Vector3.zero;
    }

    private void Start()
    {
        _playerVelocityY = Player.Velocity.y;
        float xOffset = Player.IsFacingRight ? _horizontalOffsetAmount : -_horizontalOffsetAmount;
        _targetHorizontalOffset = new Vector3(xOffset, 0.0f, 0.0f);
        _currentHorizontalOffset = _targetHorizontalOffset;
    }

    private void Update()
    {
        _playerVelocityY = Player.Velocity.y;
    }

    private void LateUpdate()
    {
        Vector3 playerPosition = Player.transform.position;
        Vector3 desiredPosition = playerPosition + HorizontalCameraOffset() + VerticalCameraOffset();
        desiredPosition.z = -1.0f;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, _smoothSpeed);
    }

    private Vector3 HorizontalCameraOffset()
    {
        float xOffset = Player.IsFacingRight ? _horizontalOffsetAmount : -_horizontalOffsetAmount;
        _targetHorizontalOffset = new Vector3(xOffset, 0.0f, 0.0f);
        _currentHorizontalOffset = Vector3.Lerp(_currentHorizontalOffset, _targetHorizontalOffset, _offsetSmoothSpeed);
        return _currentHorizontalOffset;
    }

    private Vector3 VerticalCameraOffset()
    {
        float normalizedFall = Mathf.InverseLerp(0.0f, -Player.MaxFallSpeed, _playerVelocityY);
        float yOffset = Mathf.Lerp(0.0f, _maxFallYOffset, normalizedFall);
        _targetVerticalOffset = new Vector3(0.0f, yOffset, 0.0f);
        _currentVerticalOffset = Vector3.Lerp(_currentVerticalOffset, _targetVerticalOffset, _offsetSmoothSpeed);
        return _currentVerticalOffset;
    }
}