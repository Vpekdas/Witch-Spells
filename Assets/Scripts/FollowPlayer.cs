using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private PlayerController _player;


    private void Start()
    {
        _player = FindAnyObjectByType<PlayerController>();
    }
    private void Update()
    {
        transform.position = _player.transform.position;
    }
}
