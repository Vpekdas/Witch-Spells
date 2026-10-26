using UnityEngine;

public class CameraController : MonoBehaviour
{
    private const float SMOOTH_SPEED = 0.125f;
    private const float OFFSET_SMOOTH_SPEED = 0.2f;
    private const float HORIZONTAL_OFFSET_AMOUNT = 0.5f;
    private const float MAX_FALL_Y_OFFSET = -5.0f;
    private float _playerVelocityY;
    private Vector3 _velocity;
    private Vector3 _currentHorizontalOffset;
    private Vector3 _targetHorizontalOffset;
    private Vector3 _currentVerticalOffset;
    private Vector3 _targetVerticalOffset;
    public PlayerController Player;

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
        float xOffset = Player.IsFacingRight ? HORIZONTAL_OFFSET_AMOUNT : -HORIZONTAL_OFFSET_AMOUNT;
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
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref _velocity, SMOOTH_SPEED);
    }

    private Vector3 HorizontalCameraOffset()
    {
        float xOffset = Player.IsFacingRight ? HORIZONTAL_OFFSET_AMOUNT : -HORIZONTAL_OFFSET_AMOUNT;
        _targetHorizontalOffset = new Vector3(xOffset, 0.0f, 0.0f);
        _currentHorizontalOffset = Vector3.Lerp(_currentHorizontalOffset, _targetHorizontalOffset, OFFSET_SMOOTH_SPEED);
        return _currentHorizontalOffset;
    }

    private Vector3 VerticalCameraOffset()
    {
        float normalizedFall = Mathf.InverseLerp(0.0f, -Player.MaxFallSpeed, _playerVelocityY);
        float yOffset = Mathf.Lerp(0.0f, MAX_FALL_Y_OFFSET, normalizedFall);
        _targetVerticalOffset = new Vector3(0.0f, yOffset, 0.0f);
        _currentVerticalOffset = Vector3.Lerp(_currentVerticalOffset, _targetVerticalOffset, OFFSET_SMOOTH_SPEED);
        return _currentVerticalOffset;
    }
}