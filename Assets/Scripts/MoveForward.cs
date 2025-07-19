using UnityEngine;

public class MoveForward : MonoBehaviour
{
    private ISpell _spell;

    private void Start()
    {
        _spell = GetComponent<ISpell>();
    }
    private void Update()
    {
        transform.Translate(_spell.Speed * Time.deltaTime * _spell.Direction);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}
